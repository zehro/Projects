if (scrTextBox(64, 64, "<<", clr)) {
    sub = "";
}

str[0] = "rhiannan berry";
str[1] = "project lead";
str[2] = "programming";
str[3] = "visuals";

str[4] = "kevin zhu";
str[5] = "programming";
str[6] = "music";
str[7] = "concepts";

str[8] = "fungtu atekwana";
str[9] = "visuals";
str[10] = "concepts";

str[11] = "james lee";
str[12] = "music";
str[13] = "concepts";

rainbow++;
rainbow = rainbow%255;
rb = make_colour_hsv(rainbow, 200, 255);
yPos = 64;

for(i = 0; i<14 i++) {
    if(i == 0 || i == 4 || i == 8 || i == 11) {
        yPos += 50;
        draw_set_font(fontHeading);
    } else {
        yPos += 35;
        draw_set_font(fontScore);
    }
    xPos = xCenter-(string_width(str[i]))/2;
    draw_text_colour(xPos, yPos, str[i], rb, rb, rb, rb, 1);
}

