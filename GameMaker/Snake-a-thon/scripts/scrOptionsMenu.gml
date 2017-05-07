barWidth = sprite_get_width(sprBar);
leftLim = xCenter - (barWidth/2);
rightLim  = xCenter + (barWidth/2);
txtSav = "delete save data";
boxHeight = sprite_get_height(sprWall);
//sprite_get_width


if (!instance_exists(objHandle)) {
    scrCreateSlider(xCenter, yCenter-50, global.musicVol, true);
    scrCreateSlider(xCenter, yCenter+50, global.sfxVol, false);
}

draw_text_colour(xCenter-(string_width("music")/2), yCenter-100, "music", clr, clr, clr, clr, 1);
draw_text_colour(xCenter-(string_width("sfx")/2), yCenter, "sfx", clr, clr, clr, clr, 1);

draw_rectangle_colour(xCenter-string_width(txtSav)/2 - 10, room_height-96-9, 
                    xCenter+string_width(txtSav)/2+10, room_height-96+string_height(txtSav)+7, clr, clr, clr, clr, false);
draw_rectangle_colour(xCenter-string_width(txtSav)/2 - 6, room_height-96-5, 
                    xCenter+string_width(txtSav)/2+6, room_height-96+string_height(txtSav)+3, global.color2, global.color2, global.color2, global.color2, false);
scrTextBox(xCenter-string_width(txtSav)/2, room_height-96, txtSav, clr);
if (mouse_check_button_pressed(mb_left)) {
    if (point_in_rectangle(mouse_x, mouse_y, xCenter-string_width(txtSav)/2 - 10, room_height-96-9, 
    xCenter+string_width(txtSav)/2+10, room_height-96+string_height(txtSav)+7)) {
        instance_deactivate_object(objBar);
        instance_deactivate_object(objHandle);
        confirm = true;
    }
}

if (confirm) {
    txtCon = "doing this will#delete all unlocks#and achievements.#you sure?";
    draw_rectangle_colour(200, 200, room_width-200, room_height-200, clr, clr, clr, clr, false);
    draw_rectangle_colour(204, 204, room_width-204, room_height-204, global.color2, global.color2, global.color2, global.color2, false);
    draw_set_colour(clr);
    
    draw_text(xCenter-string_width(txtCon)/2, 250, txtCon);
    
    if (scrTextBox(xCenter-string_width(txtCon)/2+50, room_height - 250 - string_height("yes"), "yes", clr) ) {
        confirm = false;
        instance_activate_object(objBar);
        instance_activate_object(objHandle);
        global.palette = 0;
        global.color1 = ds_grid_get(global.colors,0, 0);
        global.color2 = ds_grid_get(global.colors,0, 1);
        global.time = 0;
        global.move = 0;
        global.death = 0;
        global.totFood = 0;
        ini_open("save.ini");
        key = ds_map_find_first(allAch);
        for(j = 0; j < ds_map_size(allAch)-1; j++) {
            ini_write_real("achievements", string(key), 0);
            key = ds_map_find_next(allAch, key);
        }
        scrAchInit();
        ini_close();
        scrSave();
        scrChangeColor();
        
    } else if (scrTextBox(xCenter+string_width(txtCon)/2 - string_width("no") - 50, room_height - 250 - string_height("no"), "no", clr) ) {
        confirm = false;
        instance_activate_object(objBar);
        instance_activate_object(objHandle);
    }
}


if (scrTextBox(64, 64, "<<", clr) && !confirm) {
    sub = "";
    with(objHandle){instance_destroy();}
    with(objBar){instance_destroy();}
    scrSave();
}


