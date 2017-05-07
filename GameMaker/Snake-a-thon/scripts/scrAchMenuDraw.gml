vertSpace = (room_height-(2*room_height/5)+32)/9;

txtAch = "achievements";
txtStats = "stats";
txtDeaths = "deaths: "+string(global.death);
txtTime = "total time: "+string(global.time) + " s";
txtMove = "distance: "+string(global.move)+" px";
headingLen = (string_width(txtAch) +string_width(txtStats)+ 64)/2;

if(scrTextBox(64, 64, "<<", clr)) {
    sub = "";
}
draw_set_font(fontScore);
if (self.ach == "ach") {
    
    aClr = global.color2;
    sClr = global.color1;
    pageText = string(achPage+1) + " / "+ string(achPages+1);

    draw_set_colour(clr);
    draw_text(xCenter-string_width(pageText)/2, room_height - 96, pageText);
    if (achPage > 0) { //draw left arrows
        if (scrTextBox(xCenter-string_width(pageText)/2 - 16 - string_width("<"), room_height-96, "<", clr)) {
            achPage--;
        }
    }
    if (achPage <= achPages) {
        if (scrTextBox(xCenter+string_width(pageText)/2 + 16, room_height-96, ">", clr)) {
            achPage++;
        }
    }
    start = achPage*8;//0, 12, 24
    endPos = start + 8;
    if (achPage == achPages && lastAchPage != 0){
        endPos = start + lastAchPage; //convert to < bc easier
    }
    yOffset = (room_height/5 - 32);
    //set up ach list
    for (i = start+1; i <=endPos; i++) {
        xOff = 0;
        if (i>9) {
            xOff = string_width("1");
        }
        name = global.nameList[|i-1];
        yOffset += vertSpace;
        if (allAch[?name] == 1) {//all of this shit to do a strikethrough, ugh
            numOffset = string_width(string(i)+". ");
            draw_rectangle_colour(150+numOffset-xOff-2, yOffset+string_height(name)/2 - 1, 150+numOffset+string_width(name)-xOff, yOffset+string_height(name)/2 + 1, clr, clr, clr, clr, false);
        }
        draw_text_colour(150-xOff, yOffset, string(i)+". "+name, clr, clr, clr, clr, 1);
    }

} else {
    aClr = global.color1;
    sClr = global.color2;
    yOffset = (room_height/5 - 32)+vertSpace;
    
    draw_text_colour(150, yOffset, txtDeaths, clr, clr, clr, clr, 1);
    yOffset+=string_height(txtDeaths)+15;
    draw_text_colour(150, yOffset, txtTime, clr, clr, clr, clr, 1);
    yOffset+=string_height(txtTime) +15;
    draw_text_colour(150, yOffset, txtMove, clr, clr, clr, clr, 1);
    yOffset+=string_height(txtTime) +15;
    draw_text(150, yOffset, "total eaten: "+ string(global.totFood)); 
}

xAchLeft = xCenter-headingLen - 6;
xAchRight = xAchLeft + string_width(txtAch) +6;
xStatLeft = xCenter - headingLen + string_width(txtAch) +32 -6;
xStatRight = xStatLeft + string_width(txtStats) + 6;

draw_rectangle_colour(xAchLeft, 58, xAchRight, 58+string_height(txtAch)+8, sClr, sClr, sClr, sClr, false);
draw_rectangle_colour(xStatLeft, 58, xStatRight, 58+string_height(txtStats)+8, aClr, aClr, aClr, aClr, false);

if (scrTextBox(xCenter-headingLen, 64, txtAch, aClr)) {
    self.ach = "ach";
}

if (scrTextBox(xCenter-headingLen+string_width(txtAch)+32, 64, txtStats, sClr)) {
    self.ach = "stats";
}
