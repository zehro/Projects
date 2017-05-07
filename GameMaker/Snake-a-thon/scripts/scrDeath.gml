if (shield) {
    timeline_index = timShield
    timeline_position = 0
    timeline_running = true
} else if (!invincible) {
    objController.menu = "postDeathAch";
    flashlight = false;
    background_colour = global.color2;
    
    with(objHead) {instance_destroy();}
    with(objBody) {instance_destroy();}
    with(objFood) {instance_destroy();}
    if (instance_exists(objNewWall)) {
        with(objNewWall) {
            instance_destroy();
        }
    }
    global.death+=1;
    scrAchCheck();
    //instance_deactivate_object(objHead);
    //instance_deactivate_object(objBody);
    //instance_deactivate_object(objFood);
}

