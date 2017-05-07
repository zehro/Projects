﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Paint;

// BirdPaint is responsible for keeping track of all the paint-related behaviors of the bird.
// It stores what color the bird is, as well as how much of that color it has. It implements
// Paintable, so it can receive paint requests, and also sends out paint raycasts. It works
// directly with the BirdPhysics and BirdMesh, the former for paint IO and the latter for
// showing the paint effects on the bird.

namespace YeggQuest.NS_Bird
{
    public class BirdPaint : Paintable
    {
        public Bird bird;                       // The bird
        public Texture maskTexture;             // The texture that defines where paint appears on the bird
        public Texture normalTexture;           // The texture that supplies the bird's normals to the paint
        public Texture occlusionTexture;        // The texture that defines the bird's occlusion on the paint
        public LayerMask giveColorTo;           // What layers the bird can paint on

        private SphereCollider sphere;          // The sphere trigger which picks up paint requests
        private Material mat;                   // The procedural material generated by this script

        private PaintColor color;               // The current color the bird has (clear at start)
        private float colorAmmo;                // How much of that color the bird has [0-1]
        private float colorAmmoVisual;          // How much of that color the bird visually has (material)
        private float colorCutoff = 0.05f;      // The ammo value below which the Bird loses the paint
        private float colorRunoff = 0.001f;     // How quickly the paint runs out when used
        private float colorTransition = 0.3f;   // How quickly the paint transitions from color to color

        private int planeOriginID;              // shader uniform ID for the plane origin
        private int planeNormalID;              // shader uniform ID for the plane normal
        private int planeHeightID;              // shader uniform ID for the plane height
        private int colorID;                    // shader uniform ID for the color
        private int colorPrevID;                // shader uniform ID for the previous color
        private int colorTransitionID;          // shader uniform ID for the color transition

        void Start()
        {
            sphere = GetComponent<SphereCollider>();
            mat = InitializeMaterial(bird.mesh.skinnedMeshRenderer, "PaintableBird");
            mat.SetTexture("_SurfaceMaskTex", maskTexture);
            mat.SetTexture("_SurfaceNormalTex", normalTexture);
            mat.SetTexture("_SurfaceOcclusionTex", occlusionTexture);

            planeOriginID = Shader.PropertyToID("_PlaneOrigin");
            planeNormalID = Shader.PropertyToID("_PlaneNormal");
            planeHeightID = Shader.PropertyToID("_PlaneHeight");
            colorID = Shader.PropertyToID("_Color");
            colorPrevID = Shader.PropertyToID("_ColorPrev");
            colorTransitionID = Shader.PropertyToID("_ColorTransition");
        }

        void Update()
        {
            // Set the BirdPaint to the center of the bird's physics. This ensures its
            // sphere trigger is in the right place to pick up paint requests (as well
            // as send them out as raycasts.)

            transform.position = bird.physics.center.transform.position;
            transform.rotation = bird.physics.center.transform.rotation;

            // Set the plane uniforms of the paint material, so it shows how much
            // paint the bird has left on the bird itself.

            mat.SetVector(planeOriginID, bird.animator.transform.position);
            mat.SetVector(planeNormalID, Vector3.up);
            mat.SetFloat(planeHeightID, colorAmmoVisual);

            // As long as this bird has some paint of some color, it needs to raycast
            // into the world and try to affect objects with it. The strength with which
            // it does is dependent on how fast the bird is moving.

            if (color != PaintColor.Clear && colorAmmo > 0)
            {
                float strength = bird.physics.center.velocity.magnitude;
                strength = Mathf.Clamp01(Mathf.InverseLerp(0.1f, 5f, strength));
                strength *= 60 * Time.smoothDeltaTime;

                if (strength != 0)
                {
                    SendPaint(Vector3.up, strength);
                    SendPaint(Vector3.down, strength);
                    for (int y = 0; y < 8; y++)
                        for (int x = -1; x <= 1; x++)
                            SendPaint(Quaternion.Euler(x * 45, y * 45, 0) * Vector3.forward, strength);
                }
            }
        }

        void OnDestroy()
        {
            if (mat)
                Destroy(mat);
        }

        // ======================================================================================================================== MESSAGES

        // BirdPaint responds to paint requests in multiple ways. As long as it is not already
        // the given color, it does a quick animation to become that color through PaintRoutine.
        // It also refills or empties its ammo depending on whether or not it was given paint
        // or "Clear" (which is also a PaintColor.)

        public override bool Paint(PaintRequest request)
        {
            colorAmmo = request.color != PaintColor.Clear ? 1 : 0;

            if (color != request.color)
            {
                StopAllCoroutines();
                StartCoroutine(PaintRoutine(request.color));
            }

            return true;
        }

        internal void Clean()
        {
            Paint(new PaintRequest(PaintColor.Clear));
        }

        // ======================================================================================================================== HELPERS

        // A private coroutine which does a quick repainting animation. Given a new
        // color to turn, the bird turns that color over colorTransition seconds.

        // TODO: fix refilling the same color, colorAmmoVisual doesn't update because
        // color == request.color above

        private IEnumerator PaintRoutine(PaintColor color)
        {
            mat.SetColor(colorPrevID, PaintColors.ToColor(this.color));
            mat.SetColor(colorID, PaintColors.ToColor(this.color = color));

            mat.SetFloat(colorTransitionID, 0);
            float visualStart = colorAmmoVisual;

            for (float f = 0; f < colorTransition; f += Time.deltaTime)
            {
                float t = f / colorTransition;
                mat.SetFloat(colorTransitionID, t);

                if (colorAmmoVisual < colorAmmo)
                    colorAmmoVisual = Mathf.Lerp(visualStart, colorAmmo, Yutil.Smootherstep(t));

                yield return null;
            }

            colorAmmoVisual = colorAmmo;
            mat.SetFloat(colorTransitionID, 1);
        }

        // A private helper function which sends this bird's paint into the world
        // in the given direction with the given strength. Successfully painting
        // Paintable objects makes the bird slowly run out of paint.

        private void SendPaint(Vector3 direction, float strength)
        {
            // SendPaint is called back-to-back 
            if (colorAmmo <= colorCutoff || color == PaintColor.Clear)
                return;

            // Raycast and attempt to paint on a Paintable

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, direction, out hitInfo, sphere.radius, giveColorTo.value))
            {
                Paintable p = hitInfo.transform.GetComponent<Paintable>();
                if (p != null && p.Paint(new PaintRequest(hitInfo, color, strength * Mathf.Clamp01(colorAmmo * 2))))
                {
                    colorAmmo -= strength * colorRunoff;
                    colorAmmoVisual = colorAmmo;
                }
            }

            // If painting that object made the bird run out of paint,
            // the bird gives itself a paint request of Clear.

            if (colorAmmo <= colorCutoff)
                Paint(new PaintRequest(PaintColor.Clear));
        }
    }
}