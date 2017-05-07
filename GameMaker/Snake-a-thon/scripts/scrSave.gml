ini_open("save.ini");
ini_write_real("customizations","palette", global.palette);
ini_write_real("customizations","song", global.song);
ini_write_real("options", "musicVol", global.musicVol);
ini_write_real("options", "sfxVol", global.sfxVol);
ini_write_real("stats", "deaths", global.death);
ini_write_real("stats", "time", global.time);
ini_write_real("stats", "move", global.move);
ini_write_real("stats", "powerUps", global.powerUps);
ini_write_real("stats", "food", global.totFood);

//to prevent bogging down in saving (which we call a lot)
//we only aim to save the ach gotten in that run
//hence why we need achGot
for(i = 0; i < ds_list_size(achGot); i++) {
    key = achGot[| i];
    ini_write_real("achievements", string(key), 1);
    show_debug_message(key);
}
//achGot is cleared after saving
ds_list_clear(achGot);
ini_close();

newStart = true;
