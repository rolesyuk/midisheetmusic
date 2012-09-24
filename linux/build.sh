#! /bin/sh

# Script to build midisheetmusic.mono.exe on a Linux system, using Mono.NET.
# You need to install the following dependencies on Ubuntu Linux:
# sudo apt-get install mono-gmcs
# sudo apt-get install libmono-winforms2.0-cil
# sudo apt-get install alsa-utils
# sudo apt-get install timidity

gmcs -resource:NotePair.ico,MidiSheetMusic.NotePair.ico \
 -resource:treble.png,MidiSheetMusic.treble.png  \
 -resource:bass.png,MidiSheetMusic.bass.png  \
 -resource:fastforward.png,MidiSheetMusic.fastforward.png \
 -resource:play.png,MidiSheetMusic.play.png \
 -resource:pause.png,MidiSheetMusic.pause.png \
 -resource:rewind.png,MidiSheetMusic.rewind.png \
 -resource:stop.png,MidiSheetMusic.stop.png \
 -resource:volume.png,MidiSheetMusic.volume.png \
 -resource:two.png,MidiSheetMusic.two.png \
 -resource:three.png,MidiSheetMusic.three.png \
 -resource:four.png,MidiSheetMusic.four.png \
 -resource:six.png,MidiSheetMusic.six.png \
 -resource:eight.png,MidiSheetMusic.eight.png \
 -resource:nine.png,MidiSheetMusic.nine.png \
 -resource:twelve.png,MidiSheetMusic.twelve.png \
 -resource:SmallNotePair.png,MidiSheetMusic.SmallNotePair.png   \
 -resource:help.rtf,MidiSheetMusic.help.rtf \
 -resource:songs/Bach__Invention_No._13.mid,MidiSheetMusic.songs.Bach__Invention_No._13.mid \
 -resource:songs/Bach__Minuet_in_G_major.mid,MidiSheetMusic.songs.Bach__Minuet_in_G_major.mid \
 -resource:songs/Bach__Musette_in_D_major.mid,MidiSheetMusic.songs.Bach__Musette_in_D_major.mid \
 -resource:songs/Bach__Prelude_in_C_major.mid,MidiSheetMusic.songs.Bach__Prelude_in_C_major.mid \
 -resource:songs/Beethoven__Fur_Elise.mid,MidiSheetMusic.songs.Beethoven__Fur_Elise.mid \
 -resource:songs/Beethoven__Minuet_in_G_major.mid,MidiSheetMusic.songs.Beethoven__Minuet_in_G_major.mid \
 -resource:songs/Beethoven__Moonlight_Sonata.mid,MidiSheetMusic.songs.Beethoven__Moonlight_Sonata.mid \
 -resource:songs/Beethoven__Sonata_Pathetique_2nd_Mov.mid,MidiSheetMusic.songs.Beethoven__Sonata_Pathetique_2nd_Mov.mid \
 -resource:songs/Bizet__Habanera_from_Carmen.mid,MidiSheetMusic.songs.Bizet__Habanera_from_Carmen.mid \
 -resource:songs/Borodin__Polovstian_Dance.mid,MidiSheetMusic.songs.Borodin__Polovstian_Dance.mid \
 -resource:songs/Brahms__Hungarian_Dance_No._5.mid,MidiSheetMusic.songs.Brahms__Hungarian_Dance_No._5.mid \
 -resource:songs/Brahms__Waltz_No._15_in_A-flat_major.mid,MidiSheetMusic.songs.Brahms__Waltz_No._15_in_A-flat_major.mid \
 -resource:songs/Brahms__Waltz_No._9_in_D_minor.mid,MidiSheetMusic.songs.Brahms__Waltz_No._9_in_D_minor.mid \
 -resource:songs/Chopin__Minute_Waltz_Op._64_No._1_in_D-flat_major.mid,MidiSheetMusic.songs.Chopin__Minute_Waltz_Op._64_No._1_in_D-flat_major.mid \
 -resource:songs/Chopin__Nocturne_Op._9_No._1_in_B-flat_minor.mid,MidiSheetMusic.songs.Chopin__Nocturne_Op._9_No._1_in_B-flat_minor.mid \
 -resource:songs/Chopin__Nocturne_Op._9_No._2_in_E-flat_major.mid,MidiSheetMusic.songs.Chopin__Nocturne_Op._9_No._2_in_E-flat_major.mid \
 -resource:songs/Chopin__Nocturne_in_C_minor.mid,MidiSheetMusic.songs.Chopin__Nocturne_in_C_minor.mid \
 -resource:songs/Chopin__Prelude_Op._28_No._20_in_C_minor.mid,MidiSheetMusic.songs.Chopin__Prelude_Op._28_No._20_in_C_minor.mid \
 -resource:songs/Chopin__Prelude_Op._28_No._4_in_E_minor.mid,MidiSheetMusic.songs.Chopin__Prelude_Op._28_No._4_in_E_minor.mid \
 -resource:songs/Chopin__Prelude_Op._28_No._6_in_B_minor.mid,MidiSheetMusic.songs.Chopin__Prelude_Op._28_No._6_in_B_minor.mid \
 -resource:songs/Chopin__Prelude_Op._28_No._7_in_A_major.mid,MidiSheetMusic.songs.Chopin__Prelude_Op._28_No._7_in_A_major.mid \
 -resource:songs/Chopin__Waltz_Op._64_No._2_in_Csharp_minor.mid,MidiSheetMusic.songs.Chopin__Waltz_Op._64_No._2_in_Csharp_minor.mid \
 -resource:songs/Clementi__Sonatina_Op._36_No._1.mid,MidiSheetMusic.songs.Clementi__Sonatina_Op._36_No._1.mid \
 -resource:songs/Easy_Songs__Brahms_Lullaby.mid,MidiSheetMusic.songs.Easy_Songs__Brahms_Lullaby.mid \
 -resource:songs/Easy_Songs__Greensleeves.mid,MidiSheetMusic.songs.Easy_Songs__Greensleeves.mid \
 -resource:songs/Easy_Songs__Jingle_Bells.mid,MidiSheetMusic.songs.Easy_Songs__Jingle_Bells.mid \
 -resource:songs/Easy_Songs__Silent_Night.mid,MidiSheetMusic.songs.Easy_Songs__Silent_Night.mid \
 -resource:songs/Easy_Songs__Twinkle_Twinkle_Little_Star.mid,MidiSheetMusic.songs.Easy_Songs__Twinkle_Twinkle_Little_Star.mid \
 -resource:songs/Field__Nocturne_in_B-flat_major.mid,MidiSheetMusic.songs.Field__Nocturne_in_B-flat_major.mid \
 -resource:songs/Grieg__Canon_Op._38_No._8.mid,MidiSheetMusic.songs.Grieg__Canon_Op._38_No._8.mid \
 -resource:songs/Grieg__Peer_Gynt_Morning.mid,MidiSheetMusic.songs.Grieg__Peer_Gynt_Morning.mid \
 -resource:songs/Handel__Sarabande_in_D_minor.mid,MidiSheetMusic.songs.Handel__Sarabande_in_D_minor.mid \
 -resource:songs/Liadov__Prelude_Op._11_in_B_minor.mid,MidiSheetMusic.songs.Liadov__Prelude_Op._11_in_B_minor.mid \
 -resource:songs/MacDowelll__To_a_Wild_Rose.mid,MidiSheetMusic.songs.MacDowelll__To_a_Wild_Rose.mid \
 -resource:songs/Massenet__Elegy_in_E_minor.mid,MidiSheetMusic.songs.Massenet__Elegy_in_E_minor.mid \
 -resource:songs/Mendelssohn__Venetian_Boat_Song_Op._19b_No._6.mid,MidiSheetMusic.songs.Mendelssohn__Venetian_Boat_Song_Op._19b_No._6.mid \
 -resource:songs/Mendelssohn__Wedding_March.mid,MidiSheetMusic.songs.Mendelssohn__Wedding_March.mid \
 -resource:songs/Mozart__Aria_from_Don_Giovanni.mid,MidiSheetMusic.songs.Mozart__Aria_from_Don_Giovanni.mid \
 -resource:songs/Mozart__Eine_Kleine_Nachtmusik.mid,MidiSheetMusic.songs.Mozart__Eine_Kleine_Nachtmusik.mid \
 -resource:songs/Mozart__Fantasy_No._3_in_D_minor.mid,MidiSheetMusic.songs.Mozart__Fantasy_No._3_in_D_minor.mid \
 -resource:songs/Mozart__Minuet_from_Don_Juan.mid,MidiSheetMusic.songs.Mozart__Minuet_from_Don_Juan.mid \
 -resource:songs/Mozart__Rondo_Alla_Turca.mid,MidiSheetMusic.songs.Mozart__Rondo_Alla_Turca.mid \
 -resource:songs/Mozart__Sonata_K.545_in_C_major.mid,MidiSheetMusic.songs.Mozart__Sonata_K.545_in_C_major.mid \
 -resource:songs/Offenbach__Barcarolle_from_The_Tales_of_Hoffmann.mid,MidiSheetMusic.songs.Offenbach__Barcarolle_from_The_Tales_of_Hoffmann.mid \
 -resource:songs/Pachelbel__Canon_in_D_major.mid,MidiSheetMusic.songs.Pachelbel__Canon_in_D_major.mid \
 -resource:songs/Prokofiev__Peter_and_the_Wolf.mid,MidiSheetMusic.songs.Prokofiev__Peter_and_the_Wolf.mid \
 -resource:songs/Puccini__O_Mio_Babbino_Caro.mid,MidiSheetMusic.songs.Puccini__O_Mio_Babbino_Caro.mid \
 -resource:songs/Rebikov__Valse_Melancolique_Op._2_No._3.mid,MidiSheetMusic.songs.Rebikov__Valse_Melancolique_Op._2_No._3.mid \
 -resource:songs/Saint-Saens__The_Swan.mid,MidiSheetMusic.songs.Saint-Saens__The_Swan.mid \
 -resource:songs/Satie__Gnossienne_No._1.mid,MidiSheetMusic.songs.Satie__Gnossienne_No._1.mid \
 -resource:songs/Satie__Gymnopedie_No._1.mid,MidiSheetMusic.songs.Satie__Gymnopedie_No._1.mid \
 -resource:songs/Schubert__Impromptu_Op._90_No._4_in_A-flat_major.mid,MidiSheetMusic.songs.Schubert__Impromptu_Op._90_No._4_in_A-flat_major.mid \
 -resource:songs/Schubert__Moment_Musicaux_No._1_in_C_major.mid,MidiSheetMusic.songs.Schubert__Moment_Musicaux_No._1_in_C_major.mid \
 -resource:songs/Schubert__Moment_Musicaux_No._3_in_F_minor.mid,MidiSheetMusic.songs.Schubert__Moment_Musicaux_No._3_in_F_minor.mid \
 -resource:songs/Schubert__Serenade_in_D_minor.mid,MidiSheetMusic.songs.Schubert__Serenade_in_D_minor.mid \
 -resource:songs/Schumann__Scenes_From_Childhood_Op._15_No._12.mid,MidiSheetMusic.songs.Schumann__Scenes_From_Childhood_Op._15_No._12.mid \
 -resource:songs/Schumann__The_Happy_Farmer.mid,MidiSheetMusic.songs.Schumann__The_Happy_Farmer.mid \
 -resource:songs/Strauss__The_Blue_Danube_Waltz.mid,MidiSheetMusic.songs.Strauss__The_Blue_Danube_Waltz.mid \
 -resource:songs/Tchaikovsky__Album_for_the_Young_-_Old_French_Song.mid,MidiSheetMusic.songs.Tchaikovsky__Album_for_the_Young_-_Old_French_Song.mid \
 -resource:songs/Tchaikovsky__Album_for_the_Young_-_Polka.mid,MidiSheetMusic.songs.Tchaikovsky__Album_for_the_Young_-_Polka.mid \
 -resource:songs/Tchaikovsky__Album_for_the_Young_-_Waltz.mid,MidiSheetMusic.songs.Tchaikovsky__Album_for_the_Young_-_Waltz.mid \
 -resource:songs/Tchaikovsky__Nutcracker_-_Dance_of_the_Reed_Flutes.mid,MidiSheetMusic.songs.Tchaikovsky__Nutcracker_-_Dance_of_the_Reed_Flutes.mid \
 -resource:songs/Tchaikovsky__Nutcracker_-_Dance_of_the_Sugar_Plum_Fairies.mid,MidiSheetMusic.songs.Tchaikovsky__Nutcracker_-_Dance_of_the_Sugar_Plum_Fairies.mid \
 -resource:songs/Tchaikovsky__Nutcracker_-_March_of_the_Toy_Soldiers.mid,MidiSheetMusic.songs.Tchaikovsky__Nutcracker_-_March_of_the_Toy_Soldiers.mid \
 -resource:songs/Tchaikovsky__Nutcracker_-_Waltz_of_the_Flowers.mid,MidiSheetMusic.songs.Tchaikovsky__Nutcracker_-_Waltz_of_the_Flowers.mid \
 -resource:songs/Tchaikovsky__Swan_Lake.mid,MidiSheetMusic.songs.Tchaikovsky__Swan_Lake.mid \
 -resource:songs/Verdi__La_Donna_e_Mobile.mid,MidiSheetMusic.songs.Verdi__La_Donna_e_Mobile.mid \
 -win32icon:NotePair.ico -target:exe \
 -out:midisheetmusic.mono.exe \
 -reference:System.Drawing,System.Windows.Forms \
 AccidSymbol.cs BarSymbol.cs BlankSymbol.cs ChordSymbol.cs \
 ClefMeasures.cs ClefSymbol.cs KeySignature.cs \
 MidiNote.cs MidiEvent.cs MidiTrack.cs MidiFile.cs MidiFileException.cs MidiOptions.cs MidiFileReader.cs \
 MusicSymbol.cs RestSymbol.cs SheetMusic.cs  \
 MidiSheetMusic.cs Staff.cs Stem.cs SymbolWidths.cs \
 TimeSignature.cs WhiteNote.cs NoteColorDialog.cs \
 MidiPlayer.cs SheetMusicWindow.cs InstrumentDialog.cs \
 SampleSongDialog.cs Volume.cs Piano.cs TimeSigSymbol.cs PlayMeasuresDialog.cs LyricSymbol.cs

