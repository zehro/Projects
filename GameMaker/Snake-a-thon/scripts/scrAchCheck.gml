//great big switch handling ach code
//runs every frame of gameplay
//iterates through ach and sends index and value of that ach to scrAchGet
for(i = 0; i < ds_list_size(global.ach); i++) {
    key = ach[| i];
    switch(key) {
        case "him... big":
            if (score >= 50) {scrAchGet(i, key);}
            break;
        case "nom, nom, nom":
            if (global.totFood == 1) {scrAchGet(i, key);}
            break;
        case "that was quick":
            if (!instance_exists(objHead) && objController.starttime <5) {scrAchGet(i, key);}
            break;
        case "starving":
            if (objController.starttime >= 30 && score == 0) {scrAchGet(i, key);}
            break;
        case "just eat already":
            if (objController.starttime >= 60 && score == 0) {scrAchGet(i, key);}
            break;
        case "munchies":
            if (global.totFood == 30) {scrAchGet(i, key);}
            break;
        case "i think you tripped":
            if (instance_exists(objHead) && objHead.vMax <=5) {scrAchGet(i, key);}
            break;
        case "c'mon, grandma!":
            if (instance_exists(objHead) &&  objHead.vMax <=3) {scrAchGet(i, key);}
            break;
        case "time stop":
            if (instance_exists(objHead) &&  objHead.vMax <=0) {scrAchGet(i, key);}
            break;
        case "new running shoes":
            if (instance_exists(objHead) &&  objHead.vMax >= 12) {scrAchGet(i, key);}
            break;
        case "faster, faster!":
            if (instance_exists(objHead) &&  objHead.vMax >= 16) {scrAchGet(i, key);}
            break;
        case "you died":
            if (global.death == 1) {scrAchGet(i, key);}
            break;
        case "insurance liability":
            if (global.death == 20) {scrAchGet(i, key);}
            break;
        case "persistence":
            if (global.death == 50) {scrAchGet(i,key);}
            break;
        case "snek souls":
            if (global.death == 100) {scrAchGet(i,key);}
            break;
        case "casual gamer":
            if (global.time > 60) {scrAchGet(i, key);}
            break;
        case "addict":
            if (global.time > 60*5) {scrAchGet(i, key);}
            break;
        case "marathoner":
            if (global.time > 60*10) {scrAchGet(i, key);}
            break;
        case "#1 fan":
            if (global.time > 60*20) {scrAchGet(i, key);}
            break;
        case "pro snake-a-thon-er":
            if (global.time > 60*30) {scrAchGet(i, key);}
            break;
        case "you've got a problem":
            if (global.time > 60*60) {scrAchGet(i, key);}
            break;
    }
}

