spriteSet = argument[0];
if (instance_exists(objHead)) {
        objHead.image_index = spriteSet*3;
}

//show_debug_message(instance_number(objBody));

if (instance_exists(objBody)) {
        objBody.image_index = spriteSet*3 + 1;
        
        if (objHead.bodNum >= 1) {
            (objHead.tail).image_index = spriteSet*3 + 2; 
        }
}


