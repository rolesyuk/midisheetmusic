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

@interface MidiNote : NSObject <NSCopying> {
    int starttime;  /** The start time, in pulses */
    int channel;    /** The channel */
    int notenumber; /** The note, from 0 to 127. Middle C is 60 */
    int duration;   /** The duration, in pulses */
}

-(int)startTime;
-(void)setStarttime:(int)value;
-(int)channel;
-(void)setChannel:(int)value;
-(int)number;
-(void)setNumber:(int)value;
-(int)duration;
-(void)setDuration:(int)value;
-(int)endTime;
-(void)noteOff:(int)endtime;
-(id)copyWithZone:(NSZone*)zone;
-(NSString*)description;

@end

