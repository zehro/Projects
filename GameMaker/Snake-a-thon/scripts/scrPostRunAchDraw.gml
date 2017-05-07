draw_set_font(fontScore);

//temporary post death menu
//this will look better, dont worry
txtAchGet = "achievement get:";

if (scrTextBox(room_width-50-string_width("ok"), 650, "ok", clr) || ds_list_size(achGot)==0) {
    menu = "postDeath";
    return 0;
}
draw_set_font(fontHeading);
draw_text_colour(xCenter-string_width(txtAchGet)/2, 100, txtAchGet, clr, clr, clr, clr, 1);

draw_set_font(fontScore);
for (i = 0; i < ds_list_size(achGot); i++) {
    draw_text_colour(xCenter-string_width(achGot[|i])/2, 100+50+(40*i), achGot[|i], clr, clr, clr, clr, 1);
}


