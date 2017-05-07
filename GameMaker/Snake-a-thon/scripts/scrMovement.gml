ds_list_insert(path, 0,x);//adds to coordinate list
ds_list_insert(path, 1,y);
ds_list_insert(path, 2,image_angle);

segmentStart = 0;
segStartx = path[| segmentStart];
segStarty = path[| segmentStart+1];
segmentLocation = 1;
segmentEndNode = 1;
show_debug_message(segmentEndNode);

lengthToSegmentEnd = 0;

for(i = 0; i < ds_list_size(children); i++) {
    segEndx = 0;
    segEndy = 0;
    
    segDiffx = 0;
    segDiffy = 0;
    
    segLen = 0;
    
    lengthToSegmentStart = lengthToSegmentEnd;
    child = ds_list_find_value(children, i);
    
    while(point_distance(child.x, child.y, child.parent.x, child.parent.y) 
            > lengthToSegmentEnd) {
        segEndx = path[| 3*segmentEndNode];
        segEndy = path[| 3*segmentEndNode+1];
        
        segDiffx = segEndx - segStartx;
        segDiffy = segEndy - segStarty;
        segLen = point_distance(segEndx, segEndy, segStartx, segStarty);
        lengthToSegmentEnd += segLen;
        
        segmentLocation++;
        segmentEndNode = segmentLocation;
    }
    
    distanceLeft = point_distance(child.x, child.y, child.parent.x, child.parent.y) - lengthToSegmentStart;
    percentageAlongSegment = distanceLeft/segLen;
    
    child.x = segStartx + segDiffx*percentageAlongSegment;
    child.y = segStarty + segDiffy*percentageAlongSegment;
}

