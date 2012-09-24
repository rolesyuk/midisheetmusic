#! /bin/sh
# Script to build the unit tests

gmcs -debug \
   -reference:"nunit.framework.dll" \
   -target:library -out:UnitTest.dll \
   AccidSymbol.cs BarSymbol.cs BlankSymbol.cs \
   ChordSymbol.cs ClefSymbol.cs ClefMeasures.cs KeySignature.cs \
   MidiEvent.cs MidiNote.cs MidiTrack.cs MidiFile.cs MidiFileReader.cs \
   MidiFileException.cs MidiOptions.cs MusicSymbol.cs RestSymbol.cs SheetMusic.cs \
   Staff.cs Stem.cs SymbolWidths.cs TimeSignature.cs UnitTest.cs \
   WhiteNote.cs TimeSigSymbol.cs LyricSymbol.cs

