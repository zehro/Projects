slidex = argument0;
slidey = argument1;
sound = argument2;
music = argument3;

barWidth = sprite_get_width(sprBar);

leftLim = slidex - barWidth/2;
rightLim = slidex + barWidth/2;

instance_create(slidex, slidey, objBar);
handlex = (sound * barWidth) + leftLim;
handle = instance_create(handlex, slidey, objHandle);
handle.sound = sound;
handle.music = music;


