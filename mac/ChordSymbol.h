/*
 * Copyright (c) 2009-2011 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */

#import "MusicSymbol.h"
#import "WhiteNote.h"
#import "TimeSignature.h"
#import "KeySignature.h"
#import "AccidSymbol.h"
#import "Stem.h"
#import "MidiFile.h"


struct _NoteData {
    int number;             /** The Midi note number, used to determine the color */
    WhiteNote *whitenote;   /** The white note location to draw */
    NoteDuration duration;  /** The duration of the note */
    BOOL leftside;          /** Whether to draw note to the left or right of the stem */
    int accid;              /** Used to create the AccidSymbols for the chord */
};
typedef struct _NoteData NoteData;

int sortChordSymbol(id chord1, id chord2, void *unused);

@interface ChordSymbol : NSObject <MusicSymbol> {
    int clef;             /** Which clef the chord is being drawn in */
    int starttime;        /** The time (in pulses) the notes occurs at */
    int endtime;          /** The starttime plus the longest note duration */
    NoteData* notedata;   /** The notes to draw */
    int notedata_len;     /** The length of the notedata array */
    Array* accidsymbols;  /** The accidental symbols to draw */
    int width;            /** The width of the chord */
    Stem *stem1;          /** The stem of the chord. Can be nil. */
    Stem *stem2;          /** The second stem of the chord. Can be nil */
    BOOL hastwostems;     /** True if this chord has two stems */
    void *sheetmusic;     /** Used to get colors and other SheetMusic options */
}

-(id)initWithNotes:(Array*)notes andKey:(KeySignature*)key
     andTime: (TimeSignature*)time andClef:(int)c andSheet:(void*)s;
-(void) createNoteData:(Array*)notes withKey:(KeySignature*)key
               andTime:(TimeSignature*)time;

-(void)createAccidSymbols;
+(int)stemDirection:(WhiteNote*)bottom withTop:(WhiteNote*)top andClef:(int)clef;
+(BOOL)notesOverlap:(NoteData*)notedata withStart:(int)start andEnd:(int)end;
-(int)startTime;
-(int)endTime;
-(int)clef;
-(BOOL)hasTwoStems;
-(Stem*)stem;
-(int)width;
-(void)setWidth:(int)w;
-(int)minWidth;
-(int)aboveStaff;
-(int)belowStaff;
-(void)draw:(int)ytop;
-(int)drawAccid:(int)ytop;
-(void)drawNotes:(int)ytop topStaff:(WhiteNote*)topstaff;
-(void)drawNoteLetters:(int)ytop topStaff:(WhiteNote*)topstaff;
-(NSString*)letterFromNumber:(int)num andWhiteNote:(WhiteNote*)w;

+(BOOL)canCreateBeams:(Array*)chords withTime:(TimeSignature*)time 
       onBeat:(BOOL)startQuarter; 
+(void)createBeam:(Array*)chords withSpacing:(int)spacing; 
+(void)bringStemsCloser:(Array*)chords;
+(void)lineUpStemEnds:(Array*)chords;

-(NSString*)description;


@end


