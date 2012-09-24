/*
 * Copyright (c) 2007-2012 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */

#import "MidiFile.h"
#import "MidiTrack.h"
#import <Foundation/NSAutoreleasePool.h>
#include <stdlib.h>
#include <fcntl.h>
#include <string.h>
#include <errno.h>
#include <unistd.h>
#include <assert.h>
#include <stdio.h>
#include <sys/stat.h>
#include <math.h>

/** Compare two MidiNotes based on their start times.
 *  If the start times are equal, compare by their numbers.
 *  Used by the C mergesort function.
 */
int sortbytime(void* v1, void* v2) {
    MidiNote **m1 = (MidiNote**) v1;
    MidiNote **m2 = (MidiNote**) v2;
    MidiNote *note1 = *m1;
    MidiNote *note2 = *m2;

    if ([note1 startTime] == [note2 startTime]) {
        return [note1 number] - [note2 number];
    }
    else {
        return [note1 startTime] - [note2 startTime];
    }
}


/** @class MidiTrack
 * The MidiTrack takes as input the raw MidiEvents for the track, and gets:
 * - The list of midi notes in the track.
 * - The first instrument used in the track.
 *
 * For each NoteOn event in the midi file, a new MidiNote is created
 * and added to the track, using the AddNote() method.
 * 
 * The NoteOff() method is called when a NoteOff event is encountered,
 * in order to update the duration of the MidiNote.
 */ 
@implementation MidiTrack

/** Create an empty MidiTrack. Used by the copy method */
- (id)initWithTrack:(int)t {
    tracknum = t;
    notes = [Array new:20];
    instrument = 0;
    return self;
}

/** Create a MidiTrack based on the Midi events.  Extract the NoteOn/NoteOff
 *  events to gather the list of MidiNotes.
 */
- (id)initWithEvents:(Array*)list andTrack:(int)num {
    tracknum = num;
    notes = [Array new:100];
    instrument = 0;

    for (int i= 0;i < [list count]; i++) {
        MidiEvent *mevent = [list get:i];
        if ([mevent eventFlag] == EventNoteOn && [mevent velocity] > 0) {
            MidiNote *note = [[MidiNote alloc] init];
            [note setStarttime:[mevent startTime]];
            [note setChannel:[mevent channel]];
            [note setNumber:[mevent notenumber]];
            [self addNote:note];
            [note release];
        }
        else if ([mevent eventFlag] == EventNoteOn && [mevent velocity] == 0) {
            [self noteOffWithChannel:[mevent channel] andNumber:[mevent notenumber]
                  andTime:[mevent startTime] ];
        }
        else if ([mevent eventFlag] == EventNoteOff) {
            [self noteOffWithChannel:[mevent channel] andNumber:[mevent notenumber]
                  andTime:[mevent startTime] ];
        }
        else if ([mevent eventFlag] == EventProgramChange) {
            instrument = [mevent instrument];
        }
        else if ([mevent metaevent] == MetaEventLyric) {
            if (lyrics == nil) {
                lyrics = [Array new:5];
            }
            [lyrics add:mevent];
        }
    }
    if ([notes count] > 0 && [(MidiNote*)[notes get:0] channel] == 9) {
        instrument = 128;  /* Percussion */
    }
    return self;
}


- (void)dealloc {
    [notes release];
    [lyrics release];
    [super dealloc];
}

- (int)number {
    return tracknum;
}

- (void)setNumber:(int)value {
    tracknum = value;
}

- (Array*)notes {
    return notes;
}

- (NSString*)instrumentName {
    if (instrument >= 0 && instrument <= 128) {
        return [[MidiFile instrumentNames] objectAtIndex:instrument];
    }
    else {
        return @"";
    }
}


- (int)instrument {
    return instrument;
}

- (void)setInstrument:(int)value {
    instrument = value;
} 

- (Array*)lyrics {
    return lyrics;
}

-(void)setLyrics:(Array*)value {
    [lyrics release];
    lyrics = [value retain];
}

/** Add a MidiNote to this track.  This is called for each NoteOn event */
- (void)addNote:(MidiNote*)m {
    [notes add:m];
}

/** A NoteOff event occured.  Find the MidiNote of the corresponding
 * NoteOn event, and update the duration of the MidiNote.
 */
- (void)noteOffWithChannel:(int)channel andNumber:(int)number andTime:(int)endtime {
    for (int i = [notes count]-1; i >= 0; i--) {
        MidiNote* note = [notes get:i];
        if ([note channel] == channel && [note number] == number &&
            [note duration] == 0) {
            [note noteOff:endtime];
            return;
        }
    }
}

/** Return a deep copy clone of this MidiTrack */
- (id)copyWithZone:(NSZone*)zone {
    MidiTrack *track = [[MidiTrack alloc] initWithTrack:tracknum];
    [track setInstrument:instrument];
    for (int i = 0; i < [notes count]; i++) {
        MidiNote *note = [notes get:i];
        MidiNote *notecopy = [note copy];
        [[track notes] add:notecopy ];
        [notecopy release];
    }
    if (lyrics != nil) {
        Array *newlyrics = [Array new:[lyrics count]];
        for (int i = 0; i < [lyrics count]; i++) {
            MidiEvent *ev = [lyrics get:i];
            [newlyrics add:ev];
        }
        [track setLyrics:newlyrics];
        [newlyrics release];
    }
    return track;
}

- (NSString*)description {
    NSString *s = [NSString stringWithFormat:
                      @"Track number=%d instrument=%d\n", tracknum, instrument];
    for (int i = 0; i < [notes count]; i++) {
        MidiNote *m = [notes get:i];
        s = [s stringByAppendingString:[m description]];
        s = [s stringByAppendingString:@"\n"];
    }
    s = [s stringByAppendingString:@"End Track\n"];
    return s;
}

@end /* class MidiTrack */

