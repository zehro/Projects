//create a new tail piece at old tail location and set to the new "tail"
if (!(tail == self)) {
    tail.image_index = objController.spriteSet*3 + 1;
}

oldTail = tail;

//get the inverse image angle
//+180 % 360
inverseAngle = (oldTail.image_angle + 180)%360;
//32
newx = oldTail.x + (lengthdir_x(16, inverseAngle));
newy = oldTail.y + (lengthdir_y(16, inverseAngle));

newTail = instance_create(newx,newy,objBody);
//newTail.v = oldTail.v;
newTail.direction = oldTail.direction;
newTail.image_angle = oldTail.image_angle;
newTail.image_index = objController.spriteSet*3 + 2;
newTail.image_blend = global.color1;

newTail.parent = oldTail;
oldTail.child = newTail;
self.tail = newTail;
bodNum++;
