//draws the pause menu
switch(sub) {
    case "customize": scrCustomizeDraw()
                        return 0;
    case "options":  scrOptionsMenu();
                        return 0;
    default: break;

}
draw_set_font(fontScore);

txtCont = "continue";
txtRestart = "restart";
txtCustomize = "customize";
txtOpt = "options";
txtExitMenu = "main menu";
txtExit = "exit";
xOffset = -string_width(txtCustomize)/2;
yOffset = -(string_height(txtCont) + string_height(txtRestart) + string_height(txtCustomize)*2 + string_height(txtExitMenu)+string_height(txtExit) + 80)/2

if (menu == "pauseMenu") {
    //you can't do standard collisions in the draw event so this is a hacked way to do it
    if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtCont, clr)) {
        //re-actives the paused instances and makes sure colors are changed
        //scrChangeColor has to run AFTER the re-activation
        //colors can't be changed on de-activated objects
        self.menu = "";
        window_mouse_set(objMouse.xPause, objMouse.yPause);
        instance_activate_all();
        scrChangeColor();
        scrChangeSnake(self.spriteSet);
        scrSave();
        /*if (instance_exists(objHead) && objHead.flashlight == true) {
            if (bgBlend > 400) {
                background_colour = merge_colour(global.color1, global.color2, (bgBlend-400)/75);
                if (bgBlend == 475) {
                    objHead.flashlight = false;
                    bgBlend = 0;
                }
            } else {
                background_colour = global.color1;
            }
        }*/
    }
    yOffset += string_height(txtCont) + 16;
}

if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtRestart, clr)) {
    instance_activate_all();
    scrClearRoom();
    menu = "";
    scrSave();
    scrClearResults();
    scrNewSnake();
    scrNewFood(); 
}

yOffset += string_height(txtRestart) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtCustomize, clr)) {
    sub = "customize";
}

yOffset += string_height(txtExitMenu) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtOpt, clr)) {
    sub = "options";
}

yOffset += string_height(txtCustomize) + 16;

if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtExitMenu, clr)) {
    instance_activate_all();
    scrClearRoom();
    scrSave();
    scrClearResults();
    menu = "mainMenu";
    sub = "";
}

yOffset += string_height(txtOpt) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtExit, clr)) {
    game_end();
}   

