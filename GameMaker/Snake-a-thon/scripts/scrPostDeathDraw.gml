//draws the pause menu
switch(sub) {
    case "customize": scrCustomizeDraw()
                        return 0;
    default: break;

}

xCenter = room_width/2;
yCenter = room_height/2;
draw_set_font(fontScore);
clr = global.color1;
changed = false;

txtNewRun = "new run";
txtCustomize = "customize";
xOffset = -string_width(txtCustomize)/2;
yOffset = -(string_height(txtNewRun) + string_height(txtCustomize) + 16)/2

if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtNewRun, clr)) {
    //runs through restart code
    instance_activate_all();
    scrClearRoom();
    self.menu = "";
    scrSave();
    scrClearResults();
    scrNewSnake();
    scrNewFood(); 
    //scrChangeColor(); -- shouldnt be needed
}   

yOffset += string_height(txtNewRun) + 16;
if (scrTextBox(xCenter+xOffset, yCenter+yOffset, txtCustomize, clr)) {
    sub = "customize";  
}
