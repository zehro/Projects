draw_set_font(fontScore);
draw_set_color(global.color1);
draw_set_halign(fa_center);

txtNew = "lead the snake with your mouse,##but don't let him touch you!"

draw_text(xCenter, yCenter - 100, txtNew);
draw_set_halign(fa_left);
if (scrTextBox(xCenter-string_width("ok")/2, yCenter +20, "ok", global.color1)) {
    menu = "";
    sub = "";
    newStart = false;
    scrSave();
    scrNewSnake();
    scrNewFood();
}

