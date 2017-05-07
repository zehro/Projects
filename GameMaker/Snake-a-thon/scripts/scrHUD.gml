switch (menu) {
    case "mainMenu": scrMainMenuDraw(sub);
                        return 0;
    case "postDeathAch":if (objMouse.image_blend != global.color1) objMouse.image_blend = global.color1;
                        scrPostRunAchDraw();
                        break;
    case "postDeath": //"postDeath" and pause menu now go to the scrPauseDraw
    case "pauseMenu":   if (objMouse.image_blend != global.color1) objMouse.image_blend = global.color1;
                        scrPauseDraw(sub);
                        return 0;                  
}



if (instance_exists(objHead)) {
    if (objHead.flashlight == true) {
        if (menu == "") {
            bgBlend++;
            draw_circle_colour(objHead.x, objHead.y, 80, global.color2, global.color2, false);
            if (bgBlend < 30) { //fade in
                background_colour = merge_colour(global.color2, global.color1, (bgBlend)/30);
                objMouse.image_blend = merge_colour(global.color1, global.color2, (bgBlend)/30);
            } else if (bgBlend > 400) {
                background_colour = merge_colour(global.color1, global.color2, (bgBlend-400)/75);
                objMouse.image_blend = merge_colour(global.color2, global.color1, (bgBlend-400)/75);
                if (bgBlend == 475) {
                    objHead.flashlight = false;
                    objMouse.image_blend = global.color1;
                    bgBlend = 0;
                }
            } else {
                background_colour = global.color1;
                objMouse.image_blend = global.color2;
            }
        } 
    } 
    if (objHead.shield == true) {
        draw_circle_colour(objHead.x, objHead.y, 32, global.color1, global.color1, true);
    }
}
if (sub == "") {
    //this handles the score, timer, and death 
    //probably going to change to a switch statement to handle all the menus and 
    //sub menus
    draw_set_font(fontScore);
    dark = global.color1;
    txtScore = "SCORE: "+ string(self.score);
    
    //delta time is the time between each frame and needs to be converted to seconds
    if ((menu != "postDeath") && (menu != "postDeathAch") && (menu != "pauseMenu")) {
        self.starttime+=(delta_time/1000000);
    }
    
    draw_set_halign(fa_left);
    draw_set_valign(fa_top);
    draw_text_colour(50,50,txtScore,dark,dark,dark,dark,1);
    draw_text_colour(50,650,string(starttime),dark,dark,dark,dark,1);
    
}
