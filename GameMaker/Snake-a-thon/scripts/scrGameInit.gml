globalvar colors, palette, color1, color2, musicVol, sfxVol, song, songs,
delta, time, move, deaths, ach, allAch, achGot, nameList, powerUps, totFood;
global.delta = 1;
global.songs = ds_list_create();
songs[|0] = song1;
songs[|1] = spooky;
songs[|2] = dangerzone;
songs[|3] = stress;
songs[|4] = tacos;

texture_set_interpolation(false);
global.colors = ds_grid_create(18,2);

ds_grid_add(global.colors, 0,0, make_colour_rgb(191,0,255));
ds_grid_add(global.colors, 0,1, make_colour_rgb(27,27,27));

ds_grid_add(global.colors, 1,0, make_colour_rgb(0,0,0));
ds_grid_add(global.colors, 1,1, make_colour_rgb(50,50,50));

ds_grid_add(global.colors, 2,0, make_colour_rgb(204,128,230));
ds_grid_add(global.colors, 2,1, make_colour_rgb(113,207,235));

ds_grid_add(global.colors, 3,0, make_colour_rgb(107,54,0));
ds_grid_add(global.colors, 3,1, make_colour_rgb(31,153,73));

ds_grid_add(global.colors, 4,0, make_colour_rgb( 242,164, 173));
ds_grid_add(global.colors, 4,1, make_colour_rgb(137,156,214));

ds_grid_add(global.colors, 5,0, make_colour_rgb( 178,182,187));
ds_grid_add(global.colors, 5,1, make_colour_rgb(115,123,124));

ds_grid_add(global.colors, 6,0, make_colour_rgb(155,197,227));
ds_grid_add(global.colors, 6,1, make_colour_rgb(173,46,61));

ds_grid_add(global.colors, 7,0, make_colour_rgb(22,40,140));
ds_grid_add(global.colors, 7,1, make_colour_rgb(239,241,0));

ds_grid_add(global.colors, 8,0, make_colour_rgb(109, 0, 58));
ds_grid_add(global.colors, 8,1, make_colour_rgb(95,141,65));

ds_grid_add(global.colors, 9,0, make_colour_rgb(96,95,93));
ds_grid_add(global.colors, 9,1, make_colour_rgb(39,219,214));

ds_grid_add(global.colors, 10, 0, make_colour_rgb(236, 115, 87));
ds_grid_add(global.colors, 10, 1, make_colour_rgb(253, 214, 146));

ds_grid_add(global.colors, 11, 0, make_colour_rgb(0, 21, 55));
ds_grid_add(global.colors, 11, 1, make_colour_rgb(60, 27, 67));

ds_grid_add(global.colors, 12, 0, make_colour_rgb(1, 22, 56));
ds_grid_add(global.colors, 12, 1, make_colour_rgb(54, 65, 86));

ds_grid_add(global.colors, 13,0, make_colour_rgb(0, 0, 0));
ds_grid_add(global.colors, 13,1, make_colour_rgb(255,255,255));

ds_grid_add(global.colors, 14,0, make_colour_rgb(54,57,39));
ds_grid_add(global.colors, 14,1, make_colour_rgb(133,133,179));

ds_grid_add(global.colors, 15, 0, make_colour_rgb(62,42,119));
ds_grid_add(global.colors, 15, 1, make_colour_rgb(255,140,66));

ds_grid_add(global.colors, 16, 0, make_colour_rgb(76,144,63));
ds_grid_add(global.colors, 16, 1, make_colour_rgb(217,226,86));

ds_grid_add(global.colors, 17, 0, make_colour_rgb(255, 255, 255));
ds_grid_add(global.colors, 17, 1, make_colour_rgb(255,82,99));



ini_open("save.ini");
global.palette = ini_read_real("customizations", "palette", 0);
global.musicVol = ini_read_real("options", "musicVol", .5);
global.sfxVol = ini_read_real("options", "sfxVol", .5);
global.song = ini_read_real("customizations", "song", 0);
scrStatsInit();
scrAchInit();
ini_close();

global.color1 = ds_grid_get(global.colors, global.palette, 0);
global.color2 = ds_grid_get(global.colors, global.palette,1)
window_set_cursor(cr_none);
mouse = instance_create(mouse_x, mouse_y, objMouse);

mouse.image_blend = global.color2;

audio_play_sound(songs[|song], 10, true);
audio_sound_gain(songs[|song], global.musicVol, 0);
audio_sound_gain(select, global.sfxVol, 0);

