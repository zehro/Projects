
switch(sub) {
    case "customize": scrCustomizeDraw();
                        return 0;
    case "options"  : scrOptionsMenu();
                        return 0;
    case "credits"  : scrCreditsDraw();
                        return 0;
    case "ach"      : scrAchMenuDraw();
                        return 0;
    case "new"      : newStartMenu();
                        return 0;
    default: break;

}

draw_set_font(fontScore);
changed = false;

txtTitle = "snake-a-thon";
txtPlay = "play"; //starts a run
txtCust = "customize"; //opens customization sub menu
txtAch = "achievements"; //shows you acheivements gotten/their reward and possible acheivments + clear acheivement option
txtOpt = "options"; //change res, sfx, music
txtCred = "credits";
txtExit = "exit"; //exit to desktop 

xOffset =   -string_width(txtAch)/2;
yOffset =   -(string_height(txtPlay) 
            + string_height(txtCust) 
            + string_height(txtAch) 
            + string_height(txtOpt) 
            + string_height(txtCred)
            + string_height(txtExit)+80)/2

draw_set_font(fontHeading);
draw_text_colour(xCenter-(string_width(txtTitle)/2), yCenter+yOffset-64, txtTitle, clr, clr, clr, clr, 1);

draw_set_font(fontScore);

if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtPlay, clr)) {
    if (newStart) {
        sub = "new";
    } else {
        menu = "";
        scrSave();
        scrNewSnake();
        scrNewFood();
    }
}

yOffset += string_height(txtPlay) + 16;

if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtCust, clr)) {
    //open customize sub menu
    sub = "customize";
}   

yOffset += string_height(txtCust) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtAch, clr)) {
    sub = "ach";
    //open acheivments sub menu
}

yOffset += string_height(txtAch) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtOpt, clr)) {
    sub = "options";
}

yOffset += string_height(txtOpt) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtCred, clr)) {
    sub = "credits";
}

yOffset += string_height(txtCred) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtExit, clr)) {
    scrSave();
    game_end();
}
