xTarget = mouse_x;
yTarget = mouse_y;
//distance the snake will spawn away from the mouse
dist = 200;

do {
    angle = random_range(0,360);

    //get an x,y based on the random angle 
    xPos = dist*cos(angle) + xTarget;
    yPos = dist*sin(angle) + yTarget;
} until (!position_meeting(xPos, yPos, all))

head = instance_create(xPos,yPos,objHead);
head.image_speed = 0;
head.image_blend = global.color1;


//set head pos to a random xy away from mouse
