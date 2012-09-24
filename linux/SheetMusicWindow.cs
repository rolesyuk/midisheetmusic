/* Copyright (c) 2007-2011 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace MidiSheetMusic {


/** @class SheetMusicWindow
 *
 * The SheetMusicWindow is the main window/form of the application,
 * that contains the Midi Player and the Sheet Music.
 * The form supports the following menu commands
 *
 * File
 *   Open 
 *     Open a midi file, read it into a MidiFile object, and create a
 *     SheetMusic child control based on the MidiFile.  The SheetMusic 
 *     control is then displayed.
 *
 *   Open Sample Song
 *     Open one of the sample midi files that comes with MidiSheetMusic.
 *
 *   Close 
 *     Close the SheetMusic control.
 *
 *   Save As Images
 *     Save the sheet music as images (one per page)
 *
 *   Print Preview 
 *     Create a PrintPreview dialog and generate a preview of the SheetMusic.
 *     The SheetMusic.DoPrint() method handles the PrintPage callback function.
 *
 *   Print 
 *     Create a PrintDialog to print the sheet music.  
 * 
 *   Exit 
 *     Exit the application.
 *
 * View
 *   Scroll Vertically
 *     Scroll the sheet music vertically.
 *
 *   Scroll Horizontally
 *     Scroll the sheet music horizontally.
 *
 *   Zoom In
 *     Increase the zoom level on the sheet music.
 *
 *   Zoom Out
 *     Decrease the zoom level on the sheet music.
 *
 *   Zoom to 100/150%
 *     Set the zoom level to 100/150%.
 *
 *   Large/Small Notes
 *     Display large or small note sizes
 *
 * Color
 *   Enable Color
 *     Show colored notes instead of black notes
 *
 *   Choose Color
 *     Choose the colors for each note
 *
 * Tracks
 *  Track X
 *     Select which tracks of the Midi file to display
 *
 *   Use One/Two Staffs
 *     Display the Midi tracks in one staff per track, or two staffs 
 *     for all tracks.
 *
 *   Choose Instruments...
 *     Choose which instruments to use per track, when playing the sound.
 *
 * Notes
 *
 *   Key Signature
 *     Change the key signature.
 *
 *   Time Signature
 *     Change the time signature to 3/4, 4/4, etc
 *
 *   Transpose Keys
 *     Shift the note keys up or down.
 *
 *   Shift Notes
 *     Shift the notes left/right by the given number of 8th notes.
 *
 *   Measure Length
 *     Adjust the length (in pulses) of a single measure
 *
 *   Combine Notes Within
 *     Combine notes within the given millisec interval.
 *
 *   Show Note Letters
 *     In the sheet music, display the note letters (A, A#, Bb, etc)
 *     next to the notes.
 *
 *   Show Lyrics
 *     If the midi file has lyrics, display them under the notes.
 * 
 *   Show Measure Numbers.
 *     In the sheet music, display the measure numbers in the staffs.
 *
 *   Play Measures in a Loop
 *     Play the selected measures in a loop
 *
 * Help
 *   Contents
 *     Display a text area describing the MidiSheetMusic options.
 */
public class SheetMusicWindow : Form {

    MidiFile midifile;         /* The MIDI file to display */
    SheetMusic sheetmusic;     /* The Control which displays the sheet music */
    Panel scrollView;          /* The Control for scrolling the sheetmusic */
    MidiPlayer player;         /* The top panel for playing the music */
    Piano piano;               /* The piano at the top, for highlighting notes */
    PrintDocument printdoc;    /* The printer settings */
    int toPage;                /* The last page number to print */
    int currentpage;           /* The current page we are printing */
    float zoom;                /* The current zoom level (1.0 == 100%) */

    /* Color options */
    NoteColorDialog colordialog;

    /* Dialog for showing sample songs */
    SampleSongDialog sampleSongDialog;

    /* Dialog for choosing instruments */
    InstrumentDialog instrumentDialog;

    /* Dialog for playing measures in a loop */
    PlayMeasuresDialog playMeasuresDialog;

    /* Label on how to select a MIDI file */
    Label selectMidi;

    /* Menu Items */
    MenuItem openMenu;
    MenuItem openSampleSongMenu;
    MenuItem saveMenu;
    MenuItem closeMenu;
    MenuItem previewMenu;
    MenuItem printMenu;
    MenuItem exitMenu;
    MenuItem trackMenu;
    MenuItem trackDisplayMenu;
    MenuItem trackMuteMenu;
    MenuItem oneStaffMenu;
    MenuItem twoStaffMenu;
    MenuItem viewMenu;
    MenuItem scrollHorizMenu;
    MenuItem scrollVertMenu;
    MenuItem largeNotesMenu;
    MenuItem smallNotesMenu;
    MenuItem notesMenu;
    MenuItem showLettersMenu;
    MenuItem showLyricsMenu;
    MenuItem showMeasuresMenu;
    MenuItem measureMenu;
    MenuItem changeKeyMenu;
    MenuItem transposeMenu;
    MenuItem shiftNotesMenu;
    MenuItem timeSigMenu;
    MenuItem combineNotesMenu;
    MenuItem playMeasuresMenu;
    MenuItem colorMenu;
    MenuItem useColorMenu;
    MenuItem chooseColorMenu;
    MenuItem chooseInstrumentsMenu;

    /** Create a new instance of this Form.
     * This window has three child controls:
     * - The MidiPlayer
     * - The SheetMusic 
     * - The scrollView, for scrolling the SheetMusic.
     * Create the menus.
     * Create the color dialog.
     */
    public SheetMusicWindow() {

        Text = "Midi Sheet Music";
        Icon = new Icon(GetType(), "NotePair.ico");
        BackColor = Color.Gray;
        Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        Size = new Size(bounds.Width - 20, bounds.Height * 9/10);

        CreateMenu();
        DisableMenus();
        AutoScroll = false;

        if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width >= 1200) {
            zoom = 1.5f;
        }
        else {
            zoom = 1.0f;
        }

        /* Create the player panel */
        player = new MidiPlayer();
        player.Parent = this;
        player.Dock = DockStyle.Top;
        player.BackColor = Color.LightGray;
        player.BorderStyle = BorderStyle.FixedSingle;

        piano = new Piano();
        piano.Parent = this;
        // piano.BackColor = Color.LightGray;
        piano.Location = new Point(piano.Location.X, piano.Location.Y + player.Height);
        player.SetPiano(piano);
 
        scrollView = new Panel();
        scrollView.Parent = this;
        scrollView.Location = new Point(scrollView.Location.X,
                                        scrollView.Location.Y + player.Height + piano.Height);
        scrollView.BackColor = Color.Gray;
        scrollView.AutoScroll = true;
        scrollView.HorizontalScroll.Enabled = true;
        scrollView.VerticalScroll.Enabled = true;

        selectMidi = new Label();
        selectMidi.Font = new Font("Arial", 16, FontStyle.Bold);
        selectMidi.Text = "Use the menu File:Open to select a MIDI file";
        selectMidi.Dock = DockStyle.Fill;
        selectMidi.TextAlign = ContentAlignment.TopCenter;
        selectMidi.Parent = scrollView;

        colordialog = new NoteColorDialog();

        CreatePrintSettings();
        // this.Width = Math.Max(this.Width, piano.Width + SheetMusic.NoteHeight*4);
    }


    /** When the window is resized, adjust the scrollView to fill the window */
    protected override void OnResize(EventArgs e) {
        base.OnResize(e);
        if (piano != null) {
            piano.Size = new Size(player.Width, piano.Size.Height);
        }
        if (scrollView != null) {
            scrollView.Size = new Size(player.Width, ClientSize.Height - (player.Height + piano.Height));
        }
    }

    /** When the window is closing, call the Exit() function */
    protected override void OnClosed(EventArgs e) {
        base.OnClosed(e);
        this.Exit(null, null);
    }

    /** Create the MidiOptions based on the menus */
    MidiOptions GetMidiOptions() {
        MidiOptions options = new MidiOptions(midifile);

        /* Get the list of selected tracks from the Tracks menu */
        for (int track = 0; track < midifile.Tracks.Count; track++) {
            options.tracks[track] = trackDisplayMenu.MenuItems[track+2].Checked;
            options.mute[track] = trackMuteMenu.MenuItems[track+2].Checked;
        }
        options.scrollVert = scrollVertMenu.Checked;
        options.largeNoteSize = largeNotesMenu.Checked;
        options.twoStaffs = twoStaffMenu.Checked;
        options.showNoteLetters = MidiOptions.NoteNameNone;
        for (int i = 0; i < 6; i++) {
            if (showLettersMenu.MenuItems[i].Checked) {
                options.showNoteLetters = (int)showLettersMenu.MenuItems[i].Tag;
            }
        }
        if (showLyricsMenu != null) {
            options.showLyrics = showLyricsMenu.Checked;
        }
        options.showMeasures = showMeasuresMenu.Checked;
        options.shifttime = 0;
        options.transpose = 0;
        options.key = -1;
        options.time = midifile.Time;

        /* Get the time signature to use */
        foreach (MenuItem menu in timeSigMenu.MenuItems) {
            int quarter = options.time.Quarter;
            int tempo = options.time.Tempo;
            if (menu.Checked && !menu.Text.Contains("default")) {
                if (menu.Text == "3/4") {
                    options.time = new TimeSignature(3, 4, quarter, tempo);
                } else if (menu.Text == "4/4") {
                    options.time = new TimeSignature(4, 4, quarter, tempo);
                }
            }
        }

        /* Get the measure length to use */
        foreach (MenuItem menu in measureMenu.MenuItems) {
            if (menu.Checked) {
                int num = options.time.Numerator;
                int denom = options.time.Denominator;
                int measure = (int)menu.Tag;
                int tempo = options.time.Tempo;
                int quarter = measure * options.time.Quarter / options.time.Measure;
                options.time = new TimeSignature(num, denom, quarter, tempo);
            }
        } 

        /* Get the amount to shift the notes left/right */
        foreach (MenuItem menu in shiftNotesMenu.MenuItems) {
            if (menu.Checked) {
                int shift = (int)menu.Tag;
                if (shift >= 0)
                    options.shifttime = midifile.Time.Quarter/2 * (shift);
                else
                    options.shifttime = shift;
            }
        }

        /* Get the key signature to use */
        foreach (MenuItem menu in changeKeyMenu.MenuItems) {
            if (menu.Checked && !menu.Text.Contains("Default")) {
                int tag = (int)menu.Tag;
                /* If the tag is positive, it has the number of sharps.
                 * If the tag is negative, it has the number of flats.
                 */
                int num_flats = 0;
                int num_sharps = 0;
                if (tag < 0)
                    num_flats = -tag;
                else
                    num_sharps = tag;
                options.key = new KeySignature(num_sharps, num_flats).Notescale();
            }
        }

        /* Get the amount to transpose the key up/down */
        foreach (MenuItem menu in transposeMenu.MenuItems) {
            if (menu.Checked) {
                options.transpose = (int)menu.Tag;
            }
        }

        /* Get the time interval for combining notes into the same chord. */
        foreach (MenuItem menu in combineNotesMenu.MenuItems) {
            if (menu.Checked) {
                options.combineInterval = (int)menu.Tag;
            }
        }

        /* Get the list of instruments from the Instrument dialog */
        options.instruments = instrumentDialog.Instruments;
        options.useDefaultInstruments = instrumentDialog.isDefault();

        /* Get the speed/tempo to use */
        options.tempo = midifile.Time.Tempo;

        /* Get whether to play measures in a loop */
        options.playMeasuresInLoop = playMeasuresDialog.IsEnabled();
        if (options.playMeasuresInLoop) {
            options.playMeasuresInLoopStart = playMeasuresDialog.GetStartMeasure();
            options.playMeasuresInLoopEnd = playMeasuresDialog.GetEndMeasure();
            if (options.playMeasuresInLoopStart > options.playMeasuresInLoopEnd) {
                options.playMeasuresInLoopEnd = options.playMeasuresInLoopStart;
            }
        }

        /* Get the note colors to use */
        options.shadeColor = colordialog.ShadeColor;
        options.shade2Color = colordialog.Shade2Color;
        if (useColorMenu.Checked) {
            options.colors = colordialog.Colors;
        }
        else {
            options.colors = null;
        }
        return options;
    } 

    /** The Sheet Music needs to be redrawn.  Gather the sheet music
     * options from the menu items.  Then create the sheetmusic
     * control, and add it to this form. Update the MidiPlayer with
     * the new midi file.
     */
    void RedrawSheetMusic() {
        if (selectMidi != null) {
            selectMidi.Dispose();
        }

        if (sheetmusic != null) {
            sheetmusic.Dispose();
        }

        MidiOptions options = GetMidiOptions();

        /* Create a new SheetMusic Control from the midifile */
        sheetmusic = new SheetMusic(midifile, options);
        sheetmusic.SetZoom(zoom);
        sheetmusic.Parent = scrollView;

        BackColor = Color.White;
        scrollView.BackColor = Color.White;

        /* Update the Midi Player and piano */
        piano.SetMidiFile(midifile, options); 
        piano.SetShadeColors(colordialog.ShadeColor, colordialog.Shade2Color);
        player.SetMidiFile(midifile, options, sheetmusic);
    }

    /** Create the menu for this form. */
    void CreateMenu() {

        this.Menu = new MainMenu();

        CreateFileMenu();
        CreateViewMenu();
        CreateColorMenu();

        trackMenu = new MenuItem("&Tracks");
        notesMenu = new MenuItem("&Notes");
        Menu.MenuItems.Add(trackMenu);
        Menu.MenuItems.Add(notesMenu);

        CreateHelpMenu();
    }

    /** Create the File menu */
    void CreateFileMenu() {
        MenuItem filemenu = new MenuItem("&File");
        openMenu = new MenuItem("&Open...", 
                             new EventHandler(Open),
                             Shortcut.CtrlO);

        openSampleSongMenu = new MenuItem("&Open Sample Song...", 
                             new EventHandler(OpenSampleSong));

        closeMenu = new MenuItem("&Close", 
                             new EventHandler(Close),
                             Shortcut.CtrlC);

        saveMenu = new MenuItem("&Save As Images...", 
                             new EventHandler(SaveImages),
                             Shortcut.CtrlS);

        previewMenu = new MenuItem("Print Pre&view...", 
                               new EventHandler(PrintPreview));

        printMenu = new MenuItem("&Print...", 
                             new EventHandler(Print),
                             Shortcut.CtrlP);

        exitMenu = new MenuItem("E&xit", new EventHandler(Exit));

        filemenu.MenuItems.Add(openMenu);
        filemenu.MenuItems.Add(openSampleSongMenu);
        filemenu.MenuItems.Add(closeMenu);
        filemenu.MenuItems.Add(saveMenu);
        filemenu.MenuItems.Add("-");
        filemenu.MenuItems.Add(previewMenu);
        filemenu.MenuItems.Add(printMenu);
        filemenu.MenuItems.Add("-");
        filemenu.MenuItems.Add(exitMenu);

        Menu.MenuItems.Add(filemenu);
    }

    /** Create the View menu */
    void CreateViewMenu() {
        viewMenu = new MenuItem("&View");
        MenuItem zoomin = new MenuItem("Zoom In", new EventHandler(ZoomIn), Shortcut.CtrlZ);
        MenuItem zoomout = new MenuItem("Zoom Out", new EventHandler(ZoomOut), Shortcut.CtrlX);
        MenuItem zoom100 = new MenuItem("Zoom to 100%", new EventHandler(Zoom100));
        MenuItem zoom150 = new MenuItem("Zoom to 150%", new EventHandler(Zoom150));
        viewMenu.MenuItems.Add(zoomin);
        viewMenu.MenuItems.Add(zoomout);
        viewMenu.MenuItems.Add(zoom100);
        viewMenu.MenuItems.Add(zoom150);
        viewMenu.MenuItems.Add("-");

        scrollVertMenu = new MenuItem("Scroll &Vertically", new EventHandler(ScrollVertically));
        scrollHorizMenu = new MenuItem("Scroll &Horizontally", new EventHandler(ScrollHorizontally));
        scrollVertMenu.RadioCheck = true;
        scrollVertMenu.Checked = true;
        scrollHorizMenu.RadioCheck = true;
        scrollHorizMenu.Checked = false;
        viewMenu.MenuItems.Add(scrollVertMenu);
        viewMenu.MenuItems.Add(scrollHorizMenu);
        viewMenu.MenuItems.Add("-");

        smallNotesMenu = new MenuItem("&Small Notes", new EventHandler(SmallNotes));
        largeNotesMenu = new MenuItem("&Large Notes", new EventHandler(LargeNotes));
        smallNotesMenu.RadioCheck = true;
        smallNotesMenu.Checked = true;
        largeNotesMenu.RadioCheck = true;
        largeNotesMenu.Checked = false;
        viewMenu.MenuItems.Add(smallNotesMenu);
        viewMenu.MenuItems.Add(largeNotesMenu);
        Menu.MenuItems.Add(viewMenu);
    }

    /** Create the Color menu */
    void CreateColorMenu() {
        colorMenu = new MenuItem("&Color");
        useColorMenu = new MenuItem("&Use Color", new EventHandler(UseColor));
        useColorMenu.Checked = false;
        chooseColorMenu = new MenuItem("&Choose Colors...", new EventHandler(ChooseColor));
        colorMenu.MenuItems.Add(useColorMenu);
        colorMenu.MenuItems.Add(chooseColorMenu);
        Menu.MenuItems.Add(colorMenu);
    }


    /* Create the "Select Tracks to Display" menu. */
    void CreateTrackDisplayMenu() {
        MenuItem menu;
        trackDisplayMenu = new MenuItem("Select Tracks to Display");
        trackMenu.MenuItems.Add(trackDisplayMenu);

        menu = new MenuItem("Select All Tracks", new EventHandler(SelectAllTracks));
        menu.Enabled = true;
        trackDisplayMenu.MenuItems.Add(menu);

        menu = new MenuItem("Deselect All Tracks", new EventHandler(DeselectAllTracks));
        menu.Enabled = true;
        trackDisplayMenu.MenuItems.Add(menu);

        for (int i = 0; i < midifile.Tracks.Count; i++) {
            int num = i+1;
            string name = midifile.Tracks[i].InstrumentName;
            if (name  != "") {
                name = "   (" + name + ")";
            }
            menu = new MenuItem("Track " + num + name, 
                              new EventHandler(TrackSelect));
            menu.Checked = true;
            if (midifile.Tracks[i].InstrumentName == "Percussion") {
                menu.Checked = false;  // Disable percussion by default
            }
            menu.Tag = (int) i;
            trackDisplayMenu.MenuItems.Add(menu);
        }
    }

    /* Create the "Select Tracks to Mute" menu. */
    void CreateTrackMuteMenu() {
        MenuItem menu;
        trackMuteMenu = new MenuItem("Select Tracks to Mute");
        trackMenu.MenuItems.Add(trackMuteMenu);

        menu = new MenuItem("Mute All Tracks", new EventHandler(MuteAllTracks));
        menu.Enabled = true;
        trackMuteMenu.MenuItems.Add(menu);

        menu = new MenuItem("Unmute All Tracks", new EventHandler(UnmuteAllTracks));
        menu.Enabled = true;
        trackMuteMenu.MenuItems.Add(menu);

        for (int i = 0; i < midifile.Tracks.Count; i++) {
            int num = i+1;
            string name = midifile.Tracks[i].InstrumentName;
            if (name  != "") {
                name = "   (" + name + ")";
            }
            menu = new MenuItem("Track " + num + name, 
                              new EventHandler(TrackMute));
            menu.Checked = false;
            if (midifile.Tracks[i].InstrumentName == "Percussion") {
                menu.Checked = true;  // Disable percussion by default
            }
            menu.Tag = (int) i;
            trackMuteMenu.MenuItems.Add(menu);
        }
    }


    /** Create the "Track" Menu after a Midi file has been selected.
     * Add a menu item to enable/disable displaying each track.
     * Add a menu item to mute/unmute each track.
     * 
     * The MenuItem.Tag is the track number (starting at 0).
     * Add a menu item to select one staff per track.
     * Add a menu item to combine all tracks into two staffs.
     * Add a menu item to choose track instruments.
     */
    void CreateTrackMenu() {
        CreateTrackDisplayMenu();
        CreateTrackMuteMenu();
        trackMenu.MenuItems.Add("-");
        oneStaffMenu = new MenuItem("Show One Staff Per Track",
                                    new EventHandler(UseOneStaff));
        if (midifile.Tracks.Count == 1) {
            twoStaffMenu = new MenuItem("Split Track Into Two Staffs",
                                        new EventHandler(UseTwoStaffs));
            oneStaffMenu.Checked = false;
            twoStaffMenu.Checked = true;
        }
        else {
            twoStaffMenu = new MenuItem("Combine All Tracks Into Two Staffs",
                                        new EventHandler(UseTwoStaffs));
            oneStaffMenu.Checked = true;
            twoStaffMenu.Checked = false;
        }
        oneStaffMenu.RadioCheck = true;
        twoStaffMenu.RadioCheck = true;
        trackMenu.MenuItems.Add(oneStaffMenu);
        trackMenu.MenuItems.Add(twoStaffMenu);
        trackMenu.MenuItems.Add("-");
        chooseInstrumentsMenu = new MenuItem("Choose Instruments...",
                                             new EventHandler(ChooseInstruments));
        trackMenu.MenuItems.Add(chooseInstrumentsMenu);
    }

    /** Enable the "Notes" menu after a Midi file has been selected. */
    void CreateNotesMenu() {
        CreateKeySignatureMenu();
        CreateTimeSignatureMenu();
        CreateTransposeMenu();
        CreateShiftNoteMenu();
        CreateMeasureLengthMenu();
        CreateCombineNotesMenu();
        CreateShowLettersMenu();
        CreateShowLyricsMenu();
        CreateShowMeasuresMenu();
        CreatePlayMeasuresMenu();
    }

    /** Create the "Show Note Letters" sub-menu. */
    void CreateShowLettersMenu() {
        MenuItem menu;
        showLettersMenu = new MenuItem("Show Note Letters"); 

        menu = new MenuItem("None", new EventHandler(ShowNoteLetters)); 
        menu.Checked = true;
        menu.Tag = 0;
        showLettersMenu.MenuItems.Add(menu);

        menu = new MenuItem("Letters", new EventHandler(ShowNoteLetters)); 
        menu.Checked = false;
        menu.Tag = MidiOptions.NoteNameLetter;
        showLettersMenu.MenuItems.Add(menu);

        menu = new MenuItem("Fixed Do-Re-Mi", new EventHandler(ShowNoteLetters)); 
        menu.Checked = false;
        menu.Tag = MidiOptions.NoteNameFixedDoReMi;
        showLettersMenu.MenuItems.Add(menu);

        menu = new MenuItem("Movable Do-Re-Mi", new EventHandler(ShowNoteLetters)); 
        menu.Checked = false;
        menu.Tag = MidiOptions.NoteNameMovableDoReMi;
        showLettersMenu.MenuItems.Add(menu);

        menu = new MenuItem("Fixed Numbers", new EventHandler(ShowNoteLetters)); 
        menu.Checked = false;
        menu.Tag = MidiOptions.NoteNameFixedNumber;
        showLettersMenu.MenuItems.Add(menu);

        menu = new MenuItem("Movable Numbers", new EventHandler(ShowNoteLetters)); 
        menu.Checked = false;
        menu.Tag = MidiOptions.NoteNameMovableNumber;
        showLettersMenu.MenuItems.Add(menu);

        notesMenu.MenuItems.Add(showLettersMenu);
    }

    /** Create the "Show Lyrics" sub-menu. */
    void CreateShowLyricsMenu() {
        if (!midifile.HasLyrics()) {
            showLyricsMenu = null;
            return;
        }
        showLyricsMenu = new MenuItem("Show Lyrics", 
                                       new EventHandler(ShowLyrics));

        showLyricsMenu.Checked = true;
        notesMenu.MenuItems.Add(showLyricsMenu);
    }

    /** Create the "Show Measure Numbers" sub-menu. */
    void CreateShowMeasuresMenu() {
        showMeasuresMenu = new MenuItem("Show Measure Numbers", 
                                       new EventHandler(ShowMeasures));

        showMeasuresMenu.Checked = false;
        notesMenu.MenuItems.Add(showMeasuresMenu);
    }

    /** Create the "Key Signature" sub-menu.
     * Create the sub-menus for changing the key signature.
     * The Menu.Tag contains the number of sharps (if positive)
     * or the number of flats (if negative) in the key.
     */
    void CreateKeySignatureMenu() {
        MenuItem menu;
        KeySignature key;

        changeKeyMenu = new MenuItem("&Key Signature");

        /* Add the default key signature */
        menu = new MenuItem("Default", new EventHandler(ChangeKeySignature));
        menu.Checked = true;
        menu.RadioCheck = true;
        menu.Tag = 0;
        changeKeyMenu.MenuItems.Add(menu);

        /* Add the sharp key signatures */
        for (int sharps = 0; sharps <=5; sharps++) {
            key = new KeySignature (sharps, 0);
            menu = new MenuItem(key.ToString(), new EventHandler(ChangeKeySignature));
            menu.Checked = false;
            menu.RadioCheck = true;
            menu.Tag = sharps;
            changeKeyMenu.MenuItems.Add(menu);
        }

        /* Add the flat key signatures */
        for (int flats = 1; flats <=6; flats++) {
            key = new KeySignature (0, flats);
            menu = new MenuItem(key.ToString(), new EventHandler(ChangeKeySignature));
            menu.Checked = false;
            menu.RadioCheck = true;
            menu.Tag = -flats;
            changeKeyMenu.MenuItems.Add(menu);
        }
        notesMenu.MenuItems.Add(changeKeyMenu);
    }


    /** Create the "Transpose" sub-menu.
     * Create sub-menus for moving the key up or down.
     * The Menu.Tag contains the amount to shift the key by.
     */
    void CreateTransposeMenu() {
        MenuItem menu;

        transposeMenu = new MenuItem("&Transpose");
        int[] amounts = new int[] { 12, 6, 5, 4, 3, 2, 1, 0,
                                   -1, -2, -3, -4, -5, -6, -12 };

        foreach (int amount in amounts) {
            string name = "none";
            if (amount > 0)
                name = "Up " + amount;
            else if (amount < 0)
                name = "Down " + (-amount);
            menu = new MenuItem(name, new EventHandler(Transpose));
            menu.RadioCheck = true;
            if (amount == 0)
                menu.Checked = true;
            else
                menu.Checked = false;
            menu.Tag = amount;
            transposeMenu.MenuItems.Add(menu);
        }
        notesMenu.MenuItems.Add(transposeMenu);
    }

    /** Create the "Shift Note" sub-menu.
     * For the "Left to Start" sub-menu, the Menu.Tag contains the
     * time (in pulses) where the first note occurs.
     * For the "Right" sub-menus, the Menu.Tag contains the number
     * of eighth notes to shift right by.
     */
    void CreateShiftNoteMenu() {
        MenuItem menu;
        shiftNotesMenu = new MenuItem("&Shift Notes");
        menu = new MenuItem("Left to start", new EventHandler(ShiftTime));
        menu.RadioCheck = true;
        menu.Checked = false;
        shiftNotesMenu.MenuItems.Add(menu);
        int firsttime = midifile.Time.Measure * 10;
        foreach (MidiTrack t in midifile.Tracks) {
            if (firsttime > t.Notes[0].StartTime) {
                firsttime = t.Notes[0].StartTime;
            }
        }
        menu.Tag = -firsttime;
        string[] labels = new string[] {
            "none (default)", "Right 1/8 note", "Right 1/4 note", "Right 3/8 note",
            "Right 1/2 note", "Right 5/8 note", "Right 3/4 note", "Right 7/8 note"
        };
        for (int i = 0; i < 8; i++) {
            menu = new MenuItem(labels[i], new EventHandler(ShiftTime));
            menu.Checked = false;
            menu.RadioCheck = true;
            menu.Tag = i;
            shiftNotesMenu.MenuItems.Add(menu);
        }
        shiftNotesMenu.MenuItems[1].Checked = true;
        notesMenu.MenuItems.Add(shiftNotesMenu);
    }

    /** Create the Measure Length sub-menu.
     * The method MidiFile.GuessMeasureLength guesses possible values for the
     * measure length (in pulses). Create a sub-menu for each possible measure
     * length.  The Menu.Tag field contains the measure length (in pulses) for
     * each menu item.
     */
    void CreateMeasureLengthMenu() {
        MenuItem menu;
        measureMenu = new MenuItem("&Measure Length");
        menu = new MenuItem(midifile.Time.Measure + " pulses (default)",
                         new EventHandler(MeasureLength));
        menu.RadioCheck = true;
        menu.Checked = true;
        menu.Tag = midifile.Time.Measure;
        measureMenu.MenuItems.Add(menu);
        measureMenu.MenuItems.Add("-");
        List<int> lengths = midifile.GuessMeasureLength();
        foreach (int len in lengths) {
            menu = new MenuItem(len + " pulses ", new EventHandler(MeasureLength));
            menu.RadioCheck = true;
            menu.Tag = len;
            menu.Checked = false;
            measureMenu.MenuItems.Add(menu);
        }
        notesMenu.MenuItems.Add(measureMenu);
    }


    /** Create the Time Signature Menu.
     * In addition to the default time signature, add 3/4 and 4/4
     */
    void CreateTimeSignatureMenu() {
        MenuItem menu;
        timeSigMenu = new MenuItem("T&ime Signature");
        menu = new MenuItem("3/4", new EventHandler(ChangeTimeSignature));
        menu.RadioCheck = true;
        menu.Checked = false;
        timeSigMenu.MenuItems.Add(menu);
        menu =  new MenuItem("4/4", new EventHandler(ChangeTimeSignature));
        menu.RadioCheck = true;
        menu.Checked = false;
        timeSigMenu.MenuItems.Add(menu);
        if (midifile.Time.Numerator == 3 && midifile.Time.Denominator == 4) {
            timeSigMenu.MenuItems[0].Text += " (default)";
            timeSigMenu.MenuItems[0].Checked = true;
        }
        else if (midifile.Time.Numerator == 4 && midifile.Time.Denominator == 4) {
            timeSigMenu.MenuItems[1].Text += " (default)";
            timeSigMenu.MenuItems[1].Checked = true;
        }
        else {
            string name = midifile.Time.Numerator + "/" +
                          midifile.Time.Denominator + " (default)";
            timeSigMenu.MenuItems.Add(
              new MenuItem(name, new EventHandler(ChangeTimeSignature))
            );
            timeSigMenu.MenuItems[2].Checked = true;
        }
        notesMenu.MenuItems.Add(timeSigMenu);

    }

    /** Create the Combine Notes Within Interval sub-menu.
     * The method MidiFile.RoundStartTimes() is used to combine notes within
     * a given time interval (millisec) into the same chord.
     * The Menu.Tag field contains the millisecond value.
     */
    void CreateCombineNotesMenu() {
        MenuItem menu;
        combineNotesMenu = new MenuItem("&Combine Notes Within Interval");
        for (int millisec = 20; millisec <= 100; millisec += 20) {
            if (millisec == 40) { 
                menu = new MenuItem(millisec + " milliseconds (default)", new EventHandler(CombineNotes));
                menu.Checked = true;
            }
            else {
                menu = new MenuItem(millisec + " milliseconds", new EventHandler(CombineNotes));
                menu.Checked = false;
            }
            menu.RadioCheck = true;
            menu.Tag = millisec;
            combineNotesMenu.MenuItems.Add(menu);
        }
        notesMenu.MenuItems.Add(combineNotesMenu);
    }

    /** Create the "Play Measures in a Loop" sub-menu.
     * 
     */
    void CreatePlayMeasuresMenu() {
        playMeasuresMenu = new MenuItem("Play Measures in a Loop...",
                                                 new EventHandler(PlayMeasuresInLoop));
        playMeasuresMenu.Checked = false;
        notesMenu.MenuItems.Add(playMeasuresMenu);
    }


    /** Create the Help menu */
    void CreateHelpMenu() {
        MenuItem helpmenu = new MenuItem("&Help");
        MenuItem about = new MenuItem("&About...",
                                     new EventHandler(About));
        MenuItem contents = new MenuItem("&Help Contents...",
                                         new EventHandler(Help));
        helpmenu.MenuItems.Add(about);
        helpmenu.MenuItems.Add(contents);
        Menu.MenuItems.Add(helpmenu);
    }

    /** Initialize the Printer settings */
    void CreatePrintSettings() {
        printdoc = new PrintDocument();
        printdoc.PrinterSettings.FromPage = 1;
        printdoc.PrinterSettings.ToPage = printdoc.PrinterSettings.MaximumPage;
        printdoc.PrintPage += new PrintPageEventHandler(PrintPage);
        toPage = 0;
        currentpage = 1;
    }


    /** The callback function for the "Open..." menu.
     * Display a "File Open" dialog, to select a midi filename.
     * If a file is selected, call OpenMidiFile()
     */
    void Open(object obj, EventArgs args) {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter="Midi Files (*.mid)|*.mid*|All Files (*.*)|*.*";
        dialog.FilterIndex = 1;
        dialog.RestoreDirectory = false;
        if (dialog.ShowDialog() == DialogResult.OK) {
            OpenMidiFile(dialog.FileName); 
        }
    }

    /** The callback function for the "Open Sample Song..." menu.
     * Create a SampleSongDialog.  If a song is chosen, read the resource
     * file, and save it to an actual file in the temp directory.
     * Then call OpenMidiFile() using that temp filename.
     */
    void OpenSampleSong(object obj, EventArgs args) {
        if (sampleSongDialog == null) {
            sampleSongDialog = new SampleSongDialog();
        }
        if (sampleSongDialog.ShowDialog() == DialogResult.OK) {
            string name = sampleSongDialog.GetSong();
            string resourceName = "MidiSheetMusic.songs." + name + ".mid"; 
            Assembly assembly = this.GetType().Assembly;
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            try {
                string path = System.IO.Path.GetTempPath() + name + ".mid";
                FileStream tempFile = new FileStream(path, FileMode.Create);
                byte[] buf = new byte[8192];
                int len = stream.Read(buf, 0, 8192);
                while (len > 0) {
                    tempFile.Write(buf, 0, len);
                    len = stream.Read(buf, 0, 8192);
                }
                tempFile.Close();
                stream.Close();
                OpenMidiFile(path);
            }
            catch (IOException e) {
                string message = "MidiSheetMusic was unable to open the sample song: " + name;
                MessageBox.Show(message, "Error Opening File",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /**
     * Read the midi file into a MidiFile instance.
     * Create a SheetMusic control based on the MidiFile.
     * Add the sheetmusic control to this form.
     * Enable all the menu items.
     *
     * If any error occurs while reading the midi file,
     * display a MessageBox with the error message.
     */
    public void OpenMidiFile(string filename) {

        try {
            midifile = new MidiFile(filename);
            DisableMenus();
            EnableMenus();
            string displayName = Path.GetFileName(filename);
            displayName = displayName.Replace("__", ": ");
            displayName = displayName.Replace("_", " ");
            Text = displayName + " - Midi Sheet Music";
            instrumentDialog = new InstrumentDialog(midifile);
            playMeasuresDialog = new PlayMeasuresDialog(midifile);
            RedrawSheetMusic();
        }
        catch (MidiFileException e) {
            filename = Path.GetFileName(filename);
            string message = "";
            message += "MidiSheetMusic was unable to open the file " 
                       + filename;
            message += "\nIt does not appear to be a valid midi file.\n" + e.Message;

            MessageBox.Show(message, "Error Opening File", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            midifile = null;
            DisableMenus();
        }
        catch (System.IO.IOException e) {
            filename = Path.GetFileName(filename);
            string message = "";
            message += "MidiSheetMusic was unable to open the file " 
                       + filename;
            message += "because:\n" + e.Message + "\n";

            MessageBox.Show(message, "Error Opening File", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

            midifile = null;
            DisableMenus();
        }
    }

    /** The callback function for the "Close" menu.
     * When invoked this will 
     * - Dispose of the SheetMusic Control
     * - Disable the relevant menu items. 
     */
    void Close(object obj, EventArgs args) {
        if (sheetmusic == null) {
            return;
        }
        player.SetMidiFile(null, null, null);
        sheetmusic.Dispose();
        sheetmusic = null;
        DisableMenus();
        BackColor = Color.Gray;
        scrollView.BackColor = Color.Gray;
        Text = "Midi Sheet Music";
        instrumentDialog.Dispose();
        playMeasuresDialog.Dispose();
    }

    /** The callback function for the "Save As Images" menu.
     * When invoked this will save the sheet music as several
     * images, one per page.  For each page in the sheet music:
     * - Create a new bitmap, PageWidth by PageHeight
     * - Create a Graphics object for the bitmap
     * - Call the SheetMusic.DoPrint() method to draw the music
     *   onto the bitmap
     * - Save the bitmap to the file.
     */
    void SaveImages(object obj, EventArgs args) {
        if (sheetmusic == null) {
            return;
        }
        /* Stop the player so that no notes are shaded */
        player.Stop(null, null);

        /* We can only save sheet music in 'vertical scrolling' view */
        ScrollVertically(null, null);

        int numpages = sheetmusic.GetTotalPages();
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.ShowHelp = true;
        dialog.CreatePrompt = false;
        dialog.OverwritePrompt = true;
        dialog.DefaultExt = "png";
        dialog.Filter="PNG Image Files (*.png)|*.png";

        /* The initial filename in the dialog will be <midi filename>.png */
        string initname = midifile.FileName;
        initname = initname.Replace(".mid", "") + ".png";
        dialog.FileName = initname;

        if (dialog.ShowDialog() == DialogResult.OK) {
            string filename = dialog.FileName;
            if (filename.Substring(filename.Length - 4, 4) == ".png") {
                filename = filename.Substring(0, filename.Length - 4);
            }
            try {
                for (int page = 1; page <= numpages; page++) {
                    Bitmap bitmap = new Bitmap(SheetMusic.PageWidth+40,
                                               SheetMusic.PageHeight+40);
                    Graphics g = Graphics.FromImage(bitmap);
                    sheetmusic.DoPrint(g, page);
                    bitmap.Save(filename + page + ".png",
                                System.Drawing.Imaging.ImageFormat.Png);
                    g.Dispose();
                    bitmap.Dispose();
                }
            }
            catch (System.IO.IOException e) {
                string message = "";
                message += "MidiSheetMusic was unable to save to file " + 
                            filename + ".png";
                message += " because:\n" + e.Message + "\n";

                MessageBox.Show(message, "Error Saving File", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }



    /** The callback function for the "Print Preview" menu.
     * When invoked, this will spawn a PrintPreview dialog.
     * The dialog will then invoke the PrintPage() event
     * handler to actually render the pages.
     */
    void PrintPreview(object obj, EventArgs args) {
        /* Stop the player so that no notes are shaded */
        player.Stop(null, null);

        /* We can only print sheet music in 'vertical scrolling' view */
        ScrollVertically(null, null);

        currentpage = 1;
        toPage = sheetmusic.GetTotalPages();
        PrintPreviewDialog dialog = new PrintPreviewDialog();
        dialog.Document = printdoc;
        dialog.ShowDialog();
    }

    /** The callback function for the "Print..." menu.
     * When invoked, this will spawn a Print dialog.
     * The dialog will then invoke the PrintPage() event
     * handler to actually render the pages.
     */
    void Print(object obj, EventArgs args) {
        /* Stop the player so that no notes are shaded */
        player.Stop(null, null);

        /* We can only print sheet music in 'vertical scrolling' view */
        ScrollVertically(null, null);

        PrintDialog dialog = new PrintDialog();
        dialog.Document = printdoc;
        dialog.AllowSomePages = true;
        dialog.PrinterSettings.MinimumPage = 1;
        dialog.PrinterSettings.MaximumPage = sheetmusic.GetTotalPages();
        dialog.PrinterSettings.FromPage = 1;
        dialog.PrinterSettings.ToPage = dialog.PrinterSettings.MaximumPage;

        /* Reduce the margins to 0.20 inches */
        Margins margins = dialog.PrinterSettings.DefaultPageSettings.Margins;
        if (margins.Left > 20) {
            margins.Left = 20;
        }
        if (margins.Right > 20) {
            margins.Right = 20;
        }
        if (margins.Top > 20) {
            margins.Top = 20;
        }
        if (margins.Bottom > 20) {
            margins.Bottom = 20;
        }

        if (dialog.ShowDialog() == DialogResult.OK) {
            if (dialog.PrinterSettings.PrintRange == PrintRange.AllPages) {
                currentpage = 1;
                toPage = sheetmusic.GetTotalPages();
            }
            else {
                currentpage = dialog.PrinterSettings.FromPage;
                toPage = dialog.PrinterSettings.ToPage;
            }
            try {
                printdoc.Print();
            }
            catch (Exception) {}
        }
    }

    /** The callback function for the "Exit" menu.
     * Exit the application.
     */
    void Exit(object obj, EventArgs args) {
        player.Stop(null, null);
        player.DeleteSoundFile();
        this.Dispose();
        Application.Exit();
    }


    /** The callback function for the "Track <num>" menu items.
     * Update the checked status of the menu item.
     * Then, redraw the sheetmusic.
     */
    void TrackSelect(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        menu.Checked = !menu.Checked;
        RedrawSheetMusic();
    }

    /** The callback function for "Select All Tracks" menu item.
     * Check all the tracks. Then redraw the sheetmusic.
     */
    void SelectAllTracks(object obj, EventArgs args) {
       int i;
       for (i = 0; i < midifile.Tracks.Count; i++) {
           trackDisplayMenu.MenuItems[i+2].Checked = true;
       }
       RedrawSheetMusic();
    }

    /** The callback function for "Deselect All Tracks" menu item.
     * Uncheck all the tracks. Then redraw the sheetmusic.
     */
    void DeselectAllTracks(object obj, EventArgs args) {
       int i;
       for (i = 0; i < midifile.Tracks.Count; i++) {
           trackDisplayMenu.MenuItems[i+2].Checked = false;
       }
       RedrawSheetMusic();
    }

    /** The callback function for the "Mute Track <num>" menu items.
     * Update the checked status of the menu item.
     * Then, redraw the sheetmusic.
     */
    void TrackMute(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        menu.Checked = !menu.Checked;
        RedrawSheetMusic();
    }

    /** The callback function for "Mute All Tracks" menu item.
     * Check all the tracks. Then redraw the sheetmusic.
     */
    void MuteAllTracks(object obj, EventArgs args) {
       int i;
       for (i = 0; i < midifile.Tracks.Count; i++) {
           trackMuteMenu.MenuItems[i+2].Checked = true;
       }
       RedrawSheetMusic();
    }

    /** The callback function for "Unmute All Tracks" menu item.
     * Uncheck all the tracks. Then redraw the sheetmusic.
     */
    void UnmuteAllTracks(object obj, EventArgs args) {
       int i;
       for (i = 0; i < midifile.Tracks.Count; i++) {
           trackMuteMenu.MenuItems[i+2].Checked = false;
       }
       RedrawSheetMusic();
    }



    /** The callback function for the "One Staff per Track" menu.
     * Update the checked status of the menu items, and redraw
     * the sheet music.
     */
    void UseOneStaff(object obj, EventArgs args) {
        if (oneStaffMenu.Checked)
            return;
        oneStaffMenu.Checked = true;
        twoStaffMenu.Checked = false;
        RedrawSheetMusic();
    }


    /** The callback function for the "Combine/Split Into Two Staffs" menu.
     * Update the checked status of the menu item, and then
     * redraw the sheet music.
     */
    void UseTwoStaffs(object obj, EventArgs args) {
        if (twoStaffMenu.Checked)
            return;
        twoStaffMenu.Checked = true;
        oneStaffMenu.Checked = false;
        RedrawSheetMusic();
    }

    /** The callback function for the "Zoom In" menu.
     * Increase the zoom level on the sheet music.
     */
    void ZoomIn(object obj, EventArgs args) {
        if (zoom >= 4.0f) {
            return;
        }
        zoom += 0.08f;
        sheetmusic.SetZoom(zoom);
    }

    /** The callback function for the "Zoom Out" menu.
     * Increase the zoom level on the sheet music.
     */
    void ZoomOut(object obj, EventArgs args) {
        if (zoom <= 0.4f) {
            return;
        }
        zoom -= 0.08f;
        sheetmusic.SetZoom(zoom);
    }

    /** The callback function for the "Zoom to 100%" menu.
     * Set the zoom level to 100%.
     */
    void Zoom100(object obj, EventArgs args) {
        zoom = 1.0f;
        sheetmusic.SetZoom(zoom);
    }

    /** The callback function for the "Zoom to 150%" menu.
     * Set the zoom level to 150%.
     */
    void Zoom150(object obj, EventArgs args) {
        zoom = 1.5f;
        sheetmusic.SetZoom(zoom);
    }

    /** The callback function for the "Scroll Vertically" menu. */
    void ScrollVertically(object obj, EventArgs args) {
        if (scrollVertMenu.Checked)
            return;
        scrollVertMenu.Checked = true;
        scrollHorizMenu.Checked = false;
        RedrawSheetMusic();
    }

    /** The callback function for the "Scroll Horizontally" menu. */
    void ScrollHorizontally(object obj, EventArgs args) {
        if (scrollHorizMenu.Checked)
            return;
        scrollHorizMenu.Checked = true;
        scrollVertMenu.Checked = false;
        RedrawSheetMusic();
    }

    /** The callback function for the "Large Notes" menu. */
    void LargeNotes(object obj, EventArgs args) {
        if (largeNotesMenu.Checked) 
            return;
        largeNotesMenu.Checked = true;
        smallNotesMenu.Checked = false;
        RedrawSheetMusic();
    }

    /** The callback function for the "Small Notes" menu. */
    void SmallNotes(object obj, EventArgs args) {
        if (smallNotesMenu.Checked) 
            return;
        largeNotesMenu.Checked = false;
        smallNotesMenu.Checked = true;
        RedrawSheetMusic();
    }

    /** The callback function for the "Show Note Letters" menu. */
    void ShowNoteLetters(object obj, EventArgs args) {
        for (int i = 0; i < 6; i++) {
            showLettersMenu.MenuItems[i].Checked = false;
        }
        MenuItem menu = (MenuItem) obj;
        menu.Checked = true;
        if ((int)menu.Tag != 0) {
            largeNotesMenu.Checked = true;
            smallNotesMenu.Checked = false;
        }
        RedrawSheetMusic();
    }

    /** The callback function for the "Show Lyrics" menu. */
    void ShowLyrics(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        menu.Checked = !menu.Checked;
        RedrawSheetMusic();
    }

    /** The callback function for the "Show Measure Numbers" menu. */
    void ShowMeasures(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        menu.Checked = !menu.Checked;
        RedrawSheetMusic();
    }


    /** The callback function for the "Key Signature" menu. */
    void ChangeKeySignature(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        if (menu.Checked)
            return;

        foreach (MenuItem othermenu in changeKeyMenu.MenuItems) {
            othermenu.Checked = false;
        }
        menu.Checked = true;
        RedrawSheetMusic();
    }

    /** The callback function for the "Transpose" menu. */
    void Transpose(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        if (menu.Checked)
            return;

        foreach (MenuItem othermenu in transposeMenu.MenuItems) {
            othermenu.Checked = false;
        }
        menu.Checked = true;
        RedrawSheetMusic();
    }


    /** The callback function for the "Shift Notes" menu. */
    void ShiftTime(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        if (menu.Checked)
            return;

        foreach (MenuItem othermenu in shiftNotesMenu.MenuItems) {
            othermenu.Checked = false;
        }
        menu.Checked = true;
        RedrawSheetMusic();
    }

    /** The callback function for the "Time Signature" menu. */
    void ChangeTimeSignature(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        if (menu.Checked)
            return;
        foreach (MenuItem othermenu in timeSigMenu.MenuItems) {
            othermenu.Checked = false;
        }
        menu.Checked = true;

        /* The default measure length changes when we change
         * the time signature.
         */
        int defaultmeasure;
        if (menu.Text == "3/4")
            defaultmeasure = 3 * midifile.Time.Quarter;
        else if (menu.Text == "4/4")
            defaultmeasure = 4 * midifile.Time.Quarter;
        else
            defaultmeasure = midifile.Time.Measure; 
        measureMenu.MenuItems[0].Text = defaultmeasure + " pulses (default)";
        measureMenu.MenuItems[0].Tag = defaultmeasure;       
 
        RedrawSheetMusic();
    }

    /** The callback function for the "Measure Length" menu. */
    void MeasureLength(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        if (menu.Checked)
            return;
        foreach (MenuItem othermenu in measureMenu.MenuItems) {
            othermenu.Checked = false;
        }
        menu.Checked = true;
        RedrawSheetMusic();
    }

    /** The callback function for the "Combine Notes Within Interval" menu. */
    void CombineNotes(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        if (menu.Checked)
            return;

        foreach (MenuItem othermenu in combineNotesMenu.MenuItems) {
            othermenu.Checked = false;
        }
        menu.Checked = true;
        RedrawSheetMusic();
    }


    /** The callback function for the "Use Color" menu. */
    void UseColor(object obj, EventArgs args) {
        MenuItem menu = (MenuItem) obj;
        menu.Checked = !menu.Checked;
        RedrawSheetMusic();
    }

    /** The callback function for the "Choose Colors" menu */
    void ChooseColor(object obj, EventArgs args) {
        if (colordialog.ShowDialog() == DialogResult.OK) {
            RedrawSheetMusic();
        }
    }

   /** The callback function for the "Choose Instruments" menu */
   void ChooseInstruments(object obj, EventArgs args) {
       if (instrumentDialog.ShowDialog() == DialogResult.OK) {
           player.SetMidiFile(midifile, GetMidiOptions(), sheetmusic);
       }
   }
       
   /** The callback function for the "Play Measures in a Loop" menu */
   void PlayMeasuresInLoop(object obj, EventArgs args) {
       playMeasuresDialog.ShowDialog();
       playMeasuresMenu.Checked = playMeasuresDialog.IsEnabled();
       player.SetMidiFile(midifile, GetMidiOptions(), sheetmusic);
   }

   /** The callback function for the "About" menu.
    * Display the About dialog.
    */
    void About(object obj, EventArgs args) {
        Form dialog = new Form();
        dialog.Text = "About Midi Sheet Music";
        dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
        dialog.MaximizeBox = false; 
        dialog.MinimizeBox = false; 
        dialog.ShowInTaskbar = false; 

        Icon icon = new Icon(GetType(), "NotePair.ico");
        PictureBox box = new PictureBox();
        box.Image = icon.ToBitmap();
        box.Parent = dialog;
        box.Location = new Point(20, 20);
        box.Width = icon.Width;
        box.Height = icon.Height;

        Label name = new Label();
        name.Text = "Midi Sheet Music";
        name.Parent = dialog;
        name.Font = new Font("Arial", 16, FontStyle.Bold);
        name.Location = new Point(box.Location.X + box.Width + 
                                  name.Font.Height, 
                                  box.Location.Y);
        name.AutoSize = true;

        Label label = new Label();
        label.Text = "Version 2.4\nCopyright 2007-2012 Madhav Vaidyanathan\nhttp://midisheetmusic.sourceforge.net/";
        label.Parent = dialog;
        int y = Math.Max(box.Location.Y + box.Height, 
                         name.Location.Y + name.Height);
        label.Location = new Point(box.Location.X, y + label.Font.Height*2);
        label.AutoSize = true;

        dialog.Width = Math.Max(name.Location.X + name.Width,
                                     label.Location.X + label.Width) + 
                            label.Font.Height * 2;

        Button ok = new Button();
        ok.Text = "OK";
        ok.Parent = dialog;
        ok.DialogResult = DialogResult.OK;
        ok.Width = ok.Font.Height * 3;
        int x = dialog.Width/2 - ok.Width/2; 
        ok.Location = new Point(x, label.Location.Y + 
                                   label.Height + label.Font.Height);

        dialog.Height = ok.Location.Y + ok.Height + 60;
        dialog.ShowDialog();
        dialog.Dispose();
    }


    /** Callback function for the "Help Contents" Menu.
     * Display the Help Dialog.
     */
    void Help(object obj, EventArgs args) {
        Form dialog = new Form();
        dialog.Icon = new Icon(GetType(), "NotePair.ico");
        dialog.Text = "Midi Sheet Music Help Contents";
        RichTextBox box = new RichTextBox();

        Assembly assembly = this.GetType().Assembly;
        Stream stream = assembly.GetManifestResourceStream("MidiSheetMusic.help.rtf");

        box.LoadFile(stream, RichTextBoxStreamType.RichText);
        box.Parent = dialog;
        box.ReadOnly = true;
        box.Multiline = true;
        box.WordWrap = false;
        box.BackColor = Color.White;
        box.Dock = DockStyle.Fill;
        dialog.BackColor = Color.White;
        dialog.Size = new Size(600, 500);
        dialog.ShowDialog();
        dialog.Dispose();
    }

    /** The function which handles Print events from the Print Preview
     * and Print menus.  Determine which page we are printing,
     * and call the SheetMusic.DoPrint() method.  Check that the
     * Page settings are valid, and cancel the print job if they're
     * not.
     */
    void PrintPage(object obj, PrintPageEventArgs printevent) {
        if (sheetmusic == null) {
            printevent.Cancel = true;
            return;
        }

        Graphics g = printevent.Graphics;

        sheetmusic.DoPrint(g, currentpage);
        currentpage++;
        if (currentpage > toPage) {
            printevent.HasMorePages = false;
            currentpage = 1;
        }
        else {
            printevent.HasMorePages = true;
        }
    }

    /** Enable all menus once a MidiFile has been selected.
     * Enable all top-level menus (file, zoom, color, track, time, help)
     * Enable all the File menus (close, save, print, preview)
     * Create the track menu
     * Create the time menu
     * For the "Track" menu
     *   Add a menu item for each track
     *   Add the one staff/two staff menus
     * For the "Notes" menu
     *   Add the time signature menu
     *   Add the measure length menu
     *   Add the shift notes menu
     */
    void EnableMenus() {
        viewMenu.Enabled = true;
        colorMenu.Enabled = true;
        trackMenu.Enabled = true;
        notesMenu.Enabled = true;

        closeMenu.Enabled = true;
        saveMenu.Enabled = true;
        previewMenu.Enabled = true;
        printMenu.Enabled = true;

        CreateTrackMenu();
        CreateNotesMenu();

    }


    /** Disable certain menus if there is no MidiFile selected.  For
     * the Track menu, remove any sub-menus under the Track menu.
     */
    void DisableMenus() {
        viewMenu.Enabled = false;
        colorMenu.Enabled = false;
        trackMenu.Enabled = false;
        trackMenu.MenuItems.Clear();
        notesMenu.Enabled = false;
        notesMenu.MenuItems.Clear();
 
        closeMenu.Enabled = false;
        saveMenu.Enabled = false;
        previewMenu.Enabled = false;
        printMenu.Enabled = false;
    }
}

}
