//jank ass way for handling achievements

global.allAch = ds_map_create(); //a map containing all achievements and 1/0 if theyre gotten
global.ach = ds_list_create(); //the list of ach that are not gotten
global.achGot = ds_list_create(); //ach gotten during a run
global.nameList = ds_list_create(); //name of all ach
ds_list_add(nameList, 
"nom, nom, nom",//
"you died", //
"casual gamer",//
"that was quick",
"munchies",//
"starving",//
"insurance liability",//
"i think you tripped",//
"new running shoes",//
"persistence",//
"just eat already",
"c'mon, grandma!",//
"faster, faster!",//
"snek souls",//
"addict", //
"marathoner",
"him... big",
"time stop",//
"number-one fan",//
"pro snake-a-thon-er",//
"you've got a problem");//


txtAch = "achievements";

//sets up the allAch map from the name on the nameList and the data in save.ini
for (a = 0; a < ds_list_size(nameList); a++) {
    ds_map_add(allAch, nameList[|a], ini_read_real(txtAch, nameList[|a], 0));
}

//iterates through allAch(slow, so this is only done at the beginning
//if allAch key (name) maps false/0, the name is added to ach
i = 0;
key = ds_map_find_first(allAch);
for(j = 0; j < ds_map_size(allAch)-1; j++) {
    if (allAch[? key] == 0) {
        show_debug_message(key);
        ach[| i++] = key;
    }
    key = ds_map_find_next(allAch, key);
}
