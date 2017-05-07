if (instance_exists(objHead)) {
    with(objHead) {
        instance_destroy();
    }
}
if (instance_exists(objBody)) {
    with(objBody) {
        instance_destroy();
    }
}
if (instance_exists(objFood)) {
    with(objFood) {
        instance_destroy();
    }
}
if (instance_exists(objNewWall)) {
    with(objNewWall) {
        instance_destroy();
    }
    objHead.walls = 0;
}

