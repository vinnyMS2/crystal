all: MapConverter.exe

MapConverter.exe: *.cs
	mcs -out:MapConverter.exe -r:System.Drawing.Common.dll *.cs

clean:
	rm -f MapConverter.exe
