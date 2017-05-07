//argument0 x
//argument1 y
//argument2 text string
//argument3 color
//returns true if mouse in hitbox and left button pressed
ox = argument0;
oy = argument1;
txt = argument2;
clr = argument3;

draw_text_colour(ox, oy, txt, clr, clr, clr, clr, 1);
if (mouse_check_button_pressed(mb_left)) {
    if point_in_rectangle(mouse_x, mouse_y, ox, oy, ox+string_width(txt), oy+string_height(txt)) {
        audio_play_sound(select, 2, false);
        return true;
    }
}

return false;
