globalvar colors;

//manually setting up the color pallets each time
//storing in ds grid (which is basically just a 2-D array)

//first number is num pallets, second is num colors
global.colors = ds_grid_create(7,2);


ds_grid_add(colors, 0,0, make_colour_rgb(191,0,255));
ds_grid_add(colors, 0,1, make_colour_rgb(27,27,27));

ds_grid_add(colors, 1,0, make_colour_rgb(0,0,0));
ds_grid_add(colors, 1,1, make_colour_rgb(50,50,50));

ds_grid_add(colors, 2,0, make_colour_rgb(204,128,230));
ds_grid_add(colors, 2,1, make_colour_rgb(113,207,235));

ds_grid_add(colors, 3,0, make_colour_rgb(107,54,0));
ds_grid_add(colors, 3,1, make_colour_rgb(31,153,73));


ds_grid_add(colors, 4,0, make_colour_rgb(242,164,173));
ds_grid_add(colors, 4,1, make_colour_rgb(137,156,214));

ds_grid_add(colors, 5,0, make_colour_rgb(178,182,187));
ds_grid_add(colors, 5,1, make_colour_rgb(115,123,124));

ds_grid_add(colors, 6,0, make_colour_rgb(155,197,227));
ds_grid_add(colors, 6,1, make_colour_rgb(173,46,61));
