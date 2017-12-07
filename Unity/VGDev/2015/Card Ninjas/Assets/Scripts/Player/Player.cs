using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Util;
using Assets.Scripts.Grid;
using Assets.Scripts.CardSystem;

namespace Assets.Scripts.Player
{
    public class Player : Character
    {
        public delegate void NewSelectedCard(Card card, int playerIndex);
        public static event NewSelectedCard NewSelect;

        public delegate void HealthAction(float health, int playerIndex);
        public static event HealthAction UpdateHealth;

        [SerializeField]
        private Animator anim;
        [SerializeField]
        private Material[] fireClothes;
        [SerializeField]
        private Material[] earthClothes;
        [SerializeField]
        private Material[] thunderClothes;
        [SerializeField]
        private Material[] woodClothes;
        [SerializeField]
        private SkinnedMeshRenderer[] body;
        [SerializeField]
        private Weapons.Hitbox bullet;
        [SerializeField]
        private SoundPlayer sfx;
        [SerializeField]
        private GameObject Katana;
        [SerializeField]
        private GameObject WideSword;
        [SerializeField]
        private GameObject Naginata;
        [SerializeField]
        private GameObject Hammer;
        [SerializeField]
        private GameObject Fan;
        [SerializeField]
        private GameObject Kanobo;
        [SerializeField]
        private GameObject Tanto;
        [SerializeField]
        private GameObject Wakizashi;
        [SerializeField]
        private GameObject Tonfa;
        [SerializeField]
        private GameObject BoStaff;
        [SerializeField]
        private Transform barrel;
        [SerializeField]
        private int playerNumber = 1;
        [SerializeField]
        private Transform weaponPoint;

        private int damage = 0;
        private bool doOnce = false;
        private bool move = false;
        private bool useCard = false;
        private bool basicAttack = false;
        private bool attack = false;
        private bool takeDamage = false;
        private bool invun = false;
        private float invunTime = .5f;
        private float invunTimer = 0;
        private float hold = 0;//used for delays
        private Enums.Direction directionToMove;
        private GridNode nextNode;

        private float renderTime = .002f;
        private float renderTimer = 0;
        private bool animDone = false;
        private bool hit = false;
        private bool render = false;
        private PlayerStateMachine machine;
        private Enums.PlayerState prevState = 0;
        private Enums.PlayerState currState = 0;
        private Enums.Element damageElement = Enums.Element.None;
        private GameObject weapon;

        private Deck deck;
        private Hand hand;
        private const int HAND_SIZE = 4;

        private bool paused = false;
        private float animSpeed = 0;

        public Deck Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        void Awake()
        {
            GameObject dt = GameObject.Find("DeckTransfer " + playerNumber);
            if(dt != null)
            {
                deck = dt.GetComponent<UI.DeckTransfer>().Deck;
                element = dt.GetComponent<UI.DeckTransfer>().Element;
                Material[] clothes;
                switch(element)
                {
                    case Enums.Element.Fire: clothes = fireClothes; break;
                    case Enums.Element.Earth: clothes = earthClothes; break;
                    case Enums.Element.Thunder: clothes = thunderClothes; break;
                    case Enums.Element.Wood: clothes = woodClothes; break;
                    default: clothes = null; break;
                }
                if(clothes != null)
                {
                    body[0].material = clothes[0];
                    body[1].material = clothes[2];
                    body[2].material = clothes[2];
                    body[4].material = clothes[0];
                    body[5].material = clothes[0];
                    body[6].material = clothes[2];
                    body[7].material = clothes[2];
                    body[8].material = clothes[1];
                    body[9].material = clothes[1];
                    body[10].material = clothes[0];
                    body[11].material = clothes[0];
                }
            }
            else
                deck = new Deck(FindObjectOfType<CardList>().Cards);
        }
        void Start()
        {
            grid = FindObjectOfType<GridManager>().Grid;
            currentNode = grid[rowStart, colStart];
            currentNode.Owner = this;
            transform.position = currentNode.transform.position;
            hand = new Hand();
            //state machine init
            machine = new PlayerStateMachine();
            renderTimer = 0;
            invunTimer = 0;
        }

        void Update()
        {
            if (Managers.GameManager.State == Enums.GameStates.Battle && !stun)
            {
                if (paused)
                {
                    paused = false;
                    anim.speed = animSpeed;
                }
                #region detectMove
                if (CustomInput.BoolFreshPress(CustomInput.UserInput.Up, playerNumber))
                {
                    if (currentNode.panelAllowed(Enums.Direction.Up, Type))
                    {
                        directionToMove = Enums.Direction.Up;
                        nextNode = currentNode.Up;
                    }
                }
                else if (CustomInput.BoolFreshPress(CustomInput.UserInput.Down, playerNumber))
                {
                    if (currentNode.panelAllowed(Enums.Direction.Down, Type))
                    {
                        directionToMove = Enums.Direction.Down;
                        nextNode = currentNode.Down;
                    }
                }
                else if (CustomInput.BoolFreshPress(CustomInput.UserInput.Left, playerNumber))
                {
                    if (currentNode.panelAllowed(Enums.Direction.Left, Type))
                    {
                        directionToMove = Enums.Direction.Left;
                        nextNode = currentNode.Left;
                    }
                }
                else if (CustomInput.BoolFreshPress(CustomInput.UserInput.Right, playerNumber))
                {
                    if (currentNode.panelAllowed(Enums.Direction.Right, Type))
                    {
                        directionToMove = Enums.Direction.Right;
                        nextNode = currentNode.Right;
                    }
                }
                else
                    directionToMove = Enums.Direction.None;
                #endregion
                //get next state
                currState = machine.update(hit, animDone, directionToMove, hand.GetCurrentType(), hand.Empty(), playerNumber);

                //state clean up
                if (prevState != currState)
                {
                    doOnce = false;
                    animDone = false;
                    attack = false;
                    basicAttack = false;
                    move = false;
                    hit = false;
                    if (weapon != null)
                        Destroy(weapon);
                    anim.SetInteger("state", (int)currState);
                }
                if (invunTimer > 0)
                {
                    if (renderTimer > renderTime)
                    {
                        render = !render;
                        renderTimer = 0;
                        foreach (SkinnedMeshRenderer b in body)
                            b.enabled = render;
                    }
                    hit = false;
                    renderTimer += Time.deltaTime;
                    invunTimer -= Time.deltaTime;
                }
                else if(!render || invun)
                {
                    render = true;
                    foreach(SkinnedMeshRenderer b in body)
                        b.enabled = true;
                    invun = false;
                }

                //run state
                switch (currState)
                {
                    case Enums.PlayerState.Idle:Idle(); break;
                    case Enums.PlayerState.MoveBegining: MoveBegining(); break;
                    case Enums.PlayerState.MoveEnding: MoveEnding(); break;
                    case Enums.PlayerState.Hit: Hit(); break;
                    case Enums.PlayerState.Dead: Dead(); break;
                    case Enums.PlayerState.BasicAttack: BasicAttack(); break;
                    case Enums.PlayerState.HoriSwingMid: CardAnim(); break;
                    case Enums.PlayerState.VertiSwingHeavy: CardAnim(); break;
                    case Enums.PlayerState.ThrowLight: CardAnim(); break;
                    case Enums.PlayerState.ThrowMid: CardAnim(); break;
                    case Enums.PlayerState.Shoot: CardAnim(); break;
                    case Enums.PlayerState.ChiAttack: CardAnim(); break;
                    case Enums.PlayerState.ChiStationary: CardAnim(); break;
                    case Enums.PlayerState.TauntGokuStretch: Taunt(); break;
                    case Enums.PlayerState.TauntPointPoint: Taunt(); break;
                    case Enums.PlayerState.TauntThumbsDown: Taunt(); break;
                    case Enums.PlayerState.TauntWrasslemania: Taunt(); break;
                    case Enums.PlayerState.TauntYaMoves: Taunt(); break;
                }

                if (move)
                {
                    move = false;
                    currentNode.clearOccupied();
                    currentNode = nextNode;
                    currentNode.Owner = (this);
                    transform.position = currentNode.transform.position;
                }
                #region useCard
                if (useCard)
                {
                    if (!hand.Empty())
                    {
                        Enums.CardTypes type = hand.GetCurrentType();
                        if (type == Enums.CardTypes.SwordHori || type == Enums.CardTypes.SwordVert)
                        {
                            weapon = Instantiate(Katana);
                            weapon.transform.position = weaponPoint.position;
							weapon.transform.localScale = weaponPoint.localScale;
                            weapon.transform.parent = weaponPoint;
							weapon.transform.localEulerAngles = new Vector3(0,0,0);

                        }
                        if (type == Enums.CardTypes.WideSword)
                        {
                            weapon = Instantiate(WideSword);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);

                        }
                        else if (type == Enums.CardTypes.NaginataHori || type == Enums.CardTypes.NaginataVert)
                        {
                            weapon = Instantiate(Naginata);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else if (type == Enums.CardTypes.HammerHori || type == Enums.CardTypes.HammerVert)
                        {
                            weapon = Instantiate(Hammer);
							weapon.transform.position = weaponPoint.position;
							weapon.transform.localScale = weaponPoint.localScale;
							weapon.transform.parent = weaponPoint;
							weapon.transform.localEulerAngles = new Vector3(0,0,0);
                        }
                        else if (type == Enums.CardTypes.Fan)
                        {
                            weapon = Instantiate(Fan);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else if (type == Enums.CardTypes.Kanobo)
                        {
                            weapon = Instantiate(Kanobo);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale/.8f;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else if (type == Enums.CardTypes.Tanto)
                        {
                            weapon = Instantiate(Tanto);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else if (type == Enums.CardTypes.Wakizashi)
                        {
                            weapon = Instantiate(Wakizashi);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else if (type == Enums.CardTypes.Tonfa)
                        {
                            weapon = Instantiate(Tonfa);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale/.8f;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else if (type == Enums.CardTypes.BoStaff)
                        {
                            weapon = Instantiate(BoStaff);
                            weapon.transform.position = weaponPoint.position;
                            weapon.transform.localScale = weaponPoint.localScale/.5f;
                            weapon.transform.parent = weaponPoint;
                            weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        int sfxNumber = 0;
                        switch(type)
                        {
                            case Enums.CardTypes.SwordVert:  
                            case Enums.CardTypes.SwordHori: 
                            case Enums.CardTypes.WideSword: 
                            case Enums.CardTypes.NaginataVert: 
                            case Enums.CardTypes.NaginataHori: 
                            case Enums.CardTypes.HammerVert: 
                            case Enums.CardTypes.HammerHori: 
                            case Enums.CardTypes.Fan:
                            case Enums.CardTypes.Kanobo: 
                            case Enums.CardTypes.Tanto: 
                            case Enums.CardTypes.Wakizashi: 
                            case Enums.CardTypes.Tonfa: 
                            case Enums.CardTypes.BoStaff: sfxNumber = 0; break;
                            case Enums.CardTypes.ThrowLight:
                            case Enums.CardTypes.ThrowMid: 
                            case Enums.CardTypes.Shoot: sfxNumber = 2; break;
                            case Enums.CardTypes.ChiAttack:
                            case Enums.CardTypes.ChiStationary: sfxNumber = 3; break;
                            default: break;
                        }
                        sfx.PlaySong(sfxNumber);
                        useCard = false;
                        hand.UseCurrent(this, deck);
                        CardUIEvent();
                    }
                }
                #endregion

                if (basicAttack)
                {
                    basicAttack = false;
                    Weapons.Hitbox b = Instantiate(bullet);
					AddElement.AddElementByEnum(b.gameObject, element, true);
                    b.Owner = this.gameObject;
                    b.transform.position = Direction == Enums.Direction.Left ? currentNode.Left.transform.position : currentNode.Right.transform.position;
                    b.CurrentNode = Direction == Enums.Direction.Left ? currentNode.Left : currentNode.Right;
                    b.Direction = Direction;
                    if (playerNumber == 2)
                    {
                        Transform model = b.transform.GetChild(0);
                        model.localScale = new Vector3(model.localScale.x, -model.localScale.y, model.localScale.z);
                    }
                    sfx.PlaySong(2);
                }

                if (damage > 0 && takeDamage)
                {
                    takeDamage = false;
                    TakeDamage(damage, damageElement);
                    damage = 0;
                    damageElement = Enums.Element.None;
                }
                prevState = currState;
            }
            else
            {
                if (!paused)
                {
                    animSpeed = anim.speed;
                    anim.speed = 0;
                    paused = true;
                }
                if (stun)
                {
                    if ((stunTimer += Time.deltaTime) > stunTime)
                    {
                        stunTimer = 0f;
                        stun = false;
                    }
                }
            }
        }

        public void AnimDetector()
        {
            animDone = true;
        }

        public void Attack()
        {
            attack = true;
        }

        private void CardUIEvent()
        {
            if (NewSelect != null)
                NewSelect(hand.getCurrent(), playerNumber); //fire event to gui
        }

        public Hand Hand
        {
            get { return hand; }
        }

        public void AddCardsToHand(List<Card> cards)
        {
            hand.AddUnusedCards(deck);
            hand.PlayerHand = cards;
        }

        void OnTriggerEnter(Collider col)
        {
            Weapons.Hitbox hitbox = col.gameObject.GetComponent<Weapons.Hitbox>();
            if (hitbox != null && !invun)
            {
                if (hitbox.Owner == this.gameObject)
                    return;
                if (hitbox.GetType() == typeof(Weapons.Projectiles.Tornado))
                {
                    if(hitbox.Direction == Enums.Direction.Left)
                    {
                        if (currentNode.panelAllowed(Util.Enums.Direction.Left, Type))
                        {
                            currentNode = currentNode.Left;
                            transform.position = CurrentNode.transform.position;
                        }
                        else
                        {
                            if (currentNode.panelAllowed(Util.Enums.Direction.Up, Type))
                            {
                                currentNode = currentNode.Up;
                                transform.position = CurrentNode.transform.position;
                                hit = true;
                                damage = hitbox.Damage;
                                damageElement = hitbox.Element;
                            }
                            else if (currentNode.panelAllowed(Util.Enums.Direction.Down, Type))
                            {
                                currentNode = currentNode.Down;
                                transform.position = CurrentNode.transform.position;
                                hit = true;
                                damage = hitbox.Damage;
                                damageElement = hitbox.Element;
                            }
                        }
                    }
                    else
                    {
                        if (currentNode.panelAllowed(Util.Enums.Direction.Right, Type))
                        {
                            currentNode = currentNode.Right;
                            transform.position = CurrentNode.transform.position;
                        }
                        else
                        {
                            if (currentNode.panelAllowed(Util.Enums.Direction.Up, Type))
                            {
                                currentNode = currentNode.Up;
                                transform.position = CurrentNode.transform.position;
                                hit = true;
                                damage = hitbox.Damage;
                                damageElement = hitbox.Element;
                            }
                            else if (currentNode.panelAllowed(Util.Enums.Direction.Down, Type))
                            {
                                currentNode = currentNode.Down;
                                transform.position = CurrentNode.transform.position;
                                hit = true;
                                damage = hitbox.Damage;
                                damageElement = hitbox.Element;
                            }
                        }
                    }
                }
                else
                {
                    hit = true;
                    damage = hitbox.Damage;
                    damageElement = hitbox.Element;
                }
            }
            Weapons.Projectiles.Stun s = col.gameObject.GetComponent<Weapons.Projectiles.Stun>();
            if (s != null)
                Stun = true;
        }

        private void Idle()
        {
        }

        private void MoveBegining()
        {
        }

        private void MoveEnding()
        {
            if (!doOnce)
            {
                doOnce = true;
                move = true;
            }
        }

        private void Hit()
        {
            if (!doOnce)
            {
                doOnce = true;
                invunTimer = invunTime;
                invun = true;
                takeDamage = true;
            }
        }

        private void Dead()
        {
        }

        private void BasicAttack()
        {
            if (attack)
            {
                attack = false;
                basicAttack = true;
            }
        }

        private void CardAnim()
        {
            if (!doOnce)
            {
                doOnce = true;
                useCard = true;
            }
        }

        private void Taunt()
        {
        }

        public override void TakeDamage(int damage, Util.Enums.Element incommingElement)
        {
            if (!invincible)
            {
                health = health - (int)(damage * Util.Elements.GetDamageMultiplier(element, incommingElement));
                if (UpdateHealth != null) UpdateHealth(health, playerNumber);
                if (health <= 0)
                {
                    currentNode.clearOccupied();
                    if (playerNumber == 1)
                        Managers.GameManager.Player1Lose = true;
                    else
                        Managers.GameManager.Player1Win = true;
                }
            }
        }

        public override void AddHealth(int health)
        {
            base.AddHealth(health);
            if (UpdateHealth != null) UpdateHealth(this.health, playerNumber);
        }
    }
}
