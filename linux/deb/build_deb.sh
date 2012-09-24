#! /bin/sh

# Script to build an Ubuntu/Debian package for MidiSheetMusic.
# First build the midisheetmusic.mono.exe executable by running:
# cd ..
# ./build.sh
# Then run this script:
# sudo ./build_deb.sh
# This will create the package midisheetmusic-2.4-linux.ubuntu.deb

rm -rf debian/
mkdir debian
mkdir debian/usr
mkdir debian/usr/bin
mkdir debian/usr/share/
mkdir debian/usr/share/applications/
mkdir debian/usr/share/pixmaps
mkdir debian/DEBIAN
cp control debian/DEBIAN
cp copyright debian/DEBIAN
cp ../midisheetmusic.mono.exe debian/usr/bin/
cp midisheetmusic.desktop debian/usr/share/applications/
cp ../NotePair.png debian/usr/share/pixmaps/midisheetmusic.png
find debian -type d | xargs chmod 755
chmod 755 debian/usr/bin/midisheetmusic.mono.exe
chmod 644 debian/usr/share/applications/midisheetmusic.desktop
chown -R root:root debian/

dpkg-deb --build debian
mv debian.deb midisheetmusic-2.4-linux.ubuntu.deb


