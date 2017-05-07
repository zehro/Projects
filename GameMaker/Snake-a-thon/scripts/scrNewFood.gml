if (instance_exists(objFood)) { //if food is on the screen, deleted it
    with(objFood) {
        instance_destroy();
    }
}
numFood = irandom(3)+1;
cnt = 0;
while (cnt < numFood) {
    //randomly picks a sprite frame (AKA food type) for food
    whichFood = irandom(8);
    if (whichFood == 6 && objHead.walls >= 10) {
        whichFood = 0;
    }
    if (whichFood == 7 && objHead.walls == 0) {
        whichFood = 0;
    }
    //currently hard codes the width and height of the walls so it doesn't spawn inside a wall
    xPos = random_range(16+32,room_width-16-32); 
    yPos = random_range(16+32, room_height-16-32);
    
    //if x,y doesn't collide with any object, create food at x,y
    if (abs(instance_nearest(xPos, yPos, all).x - xPos) >= 8 && abs(instance_nearest(xPos, yPos, all).y - yPos) >= 8) {
        cnt++;
        food = instance_create(xPos,yPos,objFood);
        
        //keeps the food from animating through all the sprite frames
        food.image_speed = 0;
        food.image_index = whichFood;
        food.image_blend = global.color1;
    }
}
