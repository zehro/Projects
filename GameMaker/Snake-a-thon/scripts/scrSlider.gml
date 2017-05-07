//argument zero is the vol variable being changed
if((mouse_check_button(mb_left) && (position_meeting(mouse_x, mouse_y, self)) || pressed)) {
    if(!pressed) {
        pressed = true;
    }
    if ((self.x > leftLim)&&(self.x < rightLim)) {
        self.x = mouse_x;
        
        sound = (self.x - leftLim)/barWidth;
        if (sound < 0) {
            sound = 0;
        }else if (sound > 1) {
            sound = 1;
        }
        if(music) {
            global.musicVol = sound;
            audio_sound_gain(song1, sound, 0);
            audio_sound_gain(spooky, sound, 0);
            audio_sound_gain(dangerzone, sound, 0);
            audio_sound_gain(stress, sound, 0);
            audio_sound_gain(tacos, sound, 0);
        }   else {
            global.sfxVol = sound;
            audio_sound_gain(select, sound, 0);
        }
    }
    if (self.x <= leftLim){
        self.x = leftLim+1;
    } else if (self.x >= rightLim){
        self.x = rightLim-1;
    }
}
if(mouse_check_button_released(mb_left) && pressed) {
   pressed = false;
}
