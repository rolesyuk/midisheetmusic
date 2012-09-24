/*
 * Copyright (c) 2007-2011 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */

#import <Foundation/NSObject.h>
#import <Foundation/NSArray.h>
#import <Foundation/NSString.h>
#import <Foundation/NSZone.h>
#import <Foundation/NSException.h>

#import "Array.h"
#import "TimeSignature.h"
#import "MidiNote.h"

int sortbynote(void* note1, void* note2);
int sortbytime(void* note1, void* note2);

@interface MidiTrack : NSObject <NSCopying> {
    int tracknum;          /** The track number */
    Array* notes;          /** Array of Midi notes */
    int instrument;        /** Instrument for this track */
    Array* lyrics;         /** The lyrics in this track */
}
-(id)initWithTrack:(int)tracknum;
-(id)initWithEvents:(Array*)events andTrack:(int)tracknum;
-(void)dealloc;
-(int)number;
-(void)setNumber:(int)value;
-(Array*)notes;
-(NSString*)instrumentName;
-(int)instrument;
-(void)setInstrument:(int)value;
-(Array*)lyrics;
-(void)setLyrics:(Array*)value;
-(NSString*)description;
-(void)addNote:(MidiNote *)m;
-(void)noteOffWithChannel:(int)channel andNumber:(int)num andTime:(int)endtime;
-(id)copyWithZone:(NSZone *)zone;

@end

