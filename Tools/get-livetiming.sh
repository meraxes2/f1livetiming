#!/usr/bin/sh

DATA_PATH='http://live-timing.formula1.com/keyframe_%0.5d.bin'

# Get initial frame
#wget http://live-timing.formula1.com/keyframe.bin

i=0
filename=""
for((i=1;i<1024;i++));
do
  filename=`printf $DATA_PATH $i`
  wget $filename
  if [ "$?" -ne "0" ]; then
    sleep 30s
    i=$i-1
  fi
done
