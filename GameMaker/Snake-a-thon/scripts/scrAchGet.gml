ind = argument0;
key = argument1;

//adds the name to achGot list
//deletes it from ach list so it's no longer checked
//sets corresponding key val pair in allAch to 1/true
ds_list_add(achGot, key);
ds_list_delete(ach, ind);
allAch[? key] = 1;
achNote = instance_create(100, 100, objAch);
achNote.name = key;
