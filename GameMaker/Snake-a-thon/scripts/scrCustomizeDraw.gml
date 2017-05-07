//<< back
//Palletes / Snakes
//4xwhatever grid of paletes
//clicking one sets it
//eventually scrollable

horSpace = room_width/5;
vertSpace = room_height/4;
draw_set_font(fontScore);

picked = global.palette;
txtBack = "<<"
txtMusic = "music";
txtPalettes = "palettes";
//txtSnakes = "snakes";
headingLen = (string_width(txtPalettes) +string_width(txtMusic)+ 64)/2;

if (scrTextBox(64, 64, txtBack, clr)) {
    sub = "";
    if (picked != global.palette) {
        scrSave();
    }
}
if (cust == "palettes") {
    pClr = global.color2;
    sClr = global.color1;
    mClr = global.color1;
    
    xLeft = xCenter-headingLen-6;
    xRight = xLeft + string_width(txtPalettes)+8;
    draw_rectangle_colour(xLeft, 58, xRight, 58+string_height(txtPalettes)+8, clr, clr, clr, clr, false);
} else if (cust == "snakes") {
    pClr = global.color1;
    sClr = global.color2;
    mClr = global.color1;

    xLeft = xCenter-headingLen + string_width(txtPalettes) + 32 -6;
    //xRight = xLeft + string_width(txtSnakes)+8;
    draw_rectangle_colour(xLeft, 58, xRight, 58+string_height(txtSnakes)+8, clr, clr, clr, clr, false);
} else if (cust == "music") {
    pClr = global.color1;
    sClr = global.color1;
    mClr = global.color2;
    
    xLeft = xCenter-headingLen + string_width(txtPalettes) + 64 -6;
    xRight = xLeft + string_width(txtMusic)+8;
    draw_rectangle_colour(xLeft, 58, xRight, 58+string_height(txtMusic)+8, clr, clr, clr, clr, false);
    
    vertSpaceM = (room_height-(2*room_height/5)+32)/8;
    yOffsetM = (room_height/5 - 32);
    //set up ach list
    for (i = 0; i <ds_list_size(global.songs); i++) {
        name = audio_get_name(global.songs[|i]);
        yOffsetM += vertSpaceM;
        show_debug_message(yOffsetM);
        if (i == global.song) {//all of this shit to do a strikethrough, ugh
            numOffsetM = string_width(string(i)+". ");
            draw_set_colour(clr);
            draw_rectangle(150+numOffsetM-8, yOffsetM - 8, 150+numOffsetM+string_width(name)+8, yOffsetM+string_height(name)+8, false);
            draw_set_colour(global.color2);
            draw_rectangle(150+numOffsetM-4, yOffsetM - 4, 150+numOffsetM+string_width(name)+4, yOffsetM+string_height(name)+4,false);
            draw_set_colour(global.color1);
            draw_text_colour(150, yOffsetM, string(i+1)+". "+name, clr, clr, clr, clr, 1);
        } else if (scrTextBox(150, yOffsetM, string(i+1)+". "+name, clr)) {
            audio_stop_sound(global.songs[|global.song]);
            audio_play_sound(global.songs[|i], 10, true);
            global.song = i;
        }
        
    }
    cust = "music";
}
if (scrTextBox(xCenter - headingLen, 64, txtPalettes, pClr)) {
    scrSave();
    cust = "palettes";
}
//if (scrTextBox(xCenter - headingLen+string_width(txtPalettes)+32, 64, txtSnakes, sClr)) {
//    cust = "snakes";
//}
if (scrTextBox(xCenter - headingLen +string_width(txtPalettes)+64, 64, txtMusic, mClr)) {
    scrSave();
    cust = "music";
}


if (cust == "palettes") {
    //divide room_x by five to get space b/t squares
    //iterate through ds grid
    
    pageText = string(palettePage+1) + " / "+ string(palettePages+1);
    draw_text_colour(xCenter-string_width(pageText)/2, room_height - 96, pageText, clr, clr, clr, clr, 1);
    if (palettePage > 0) { //draw left arrows
        if (scrTextBox(xCenter-string_width(pageText)/2 - 16 - string_width("<"), room_height-96, "<", clr)) {
            palettePage--;
        }
    }
    if (palettePage <= palettePages) {
        if (scrTextBox(xCenter+string_width(pageText)/2 + 16, room_height-96, ">", clr)) {
            palettePage++;
        }
    }
    start = palettePage*12;//0, 12, 24
    endPos = start + 12;
    if (palettePage == palettePages && lastPage != 0){
        endPos = start + lastPage; //convert to < bc easier
    }
    draw_set_font(fontHeading);
    
    ytop = room_height/4;
    cnt = 0;

    for (i = start; i < endPos; i++) {

        dep = 0;
        clr1 = ds_grid_get(global.colors,i, 0);
        clr2 = ds_grid_get(global.colors,i, 1);
        if (cnt == 4) {
            dep++;
            ytop += horSpace-32;
            cnt = 0;
        }
        if (i >= 4) {
            j = i%4;
        } else {
            j = i;
        }
        
        if ( i != 0 && allAch[? nameList[|i-1]] == 0) {//i-1 ach is not unlocked, draw ??? instead
            scrTextBox(horSpace*(1+j) - (string_width("?")/2), ytop + 32 - string_height("?")/2, "?", clr);
        
        } else {
            draw_rectangle_colour(horSpace*(1+j)-32,ytop, horSpace*(1+j)+32, ytop+64, clr1, clr1, clr1, clr1, false);
            draw_rectangle_colour(horSpace*(1+j)-16,ytop+16, horSpace*(1+j)+16, ytop+48, clr2, clr2, clr2, clr2, false);
        
            if(mouse_check_button_pressed(mb_left)) {
                if (point_in_rectangle(mouse_x, mouse_y, horSpace*(1+j)-32,ytop, horSpace*(1+j)+32, ytop+64,)) {
                     picked = i;
                     global.palette = i;
                     global.color1 = clr1;//snake walls etc
                     global.color2 = clr2;//background
                     scrChangeColor();
                }   
            }
        }
        cnt++;
    }
} /*else if (cust == "snakes"){
    sets = sprite_get_number(sprSnake)/3;
    //i+=3
    ytop = 150;
    dep = 0;
    cnt = 0;
    for(i = 0; i < sets; i++) {
        //draw sub img     
        if (cnt == 4) {
            dep++;
            ytop += horSpace;
            cnt = 0;
        }
        if (i >= 4) {
            j = i - 4;
        } else {
            j = i;
        }
        draw_rectangle(horSpace*(1+j)-16,ytop, horSpace*(1+j)+16, ytop+32, clr);
        draw_sprite(sprSnake, i*3, horSpace*(j+1), ytop+16);
        if(mouse_check_button_pressed(mb_left)) {
            
            if (point_in_rectangle(mouse_x, mouse_y, horSpace*(1+j)-16,ytop, horSpace*(1+j)+16, ytop+32)) {
                 objController.spriteSet = i;
            }   
        }
        cnt++;
    
    } 

} */
