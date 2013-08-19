#pragma once
class Constants
{
public:
	Constants(void);
	~Constants(void);
};

namespace Consts 
{
	extern DWORD ptrPrintName;
	extern DWORD ptrPrintFPS;
	extern DWORD ptrShowFPS;
	extern DWORD ptrNopFPS;
	extern DWORD ptrFuncParser;
	extern DWORD ptrCallGetNextPacket;
    extern DWORD ptrAddRecStream;
	extern DWORD PrintText;
	extern DWORD DrawItem;
}

struct NormalText
{
	char* text;
	int r,g,b;
	int x,y;
	int font;
	char *TextName;
}; 

// Display Creature Text Structure
struct PlayerText
{
	char *DisplayText;
	char *CreatureName;
	int CreatureId;
	int cR;
	int cG;
	int cB;
	int TextFont;
	int RelativeX;
	int RelativeY;

};

// Icon Structure
struct Icon
{
	int IconId;
	int X;
	int Y;
	int BitmapSize;
	int ItemId;
	int ItemCount;
	int TextFont;
	int cR;
	int cG;
	int cB;
};

// Skin Structure
struct Skin
{
	int SkinId;
	int X;
	int Y;
	int Width;
	int Height;
	int GUIId;
};

extern CRITICAL_SECTION NormalTextCriticalSection;
extern CRITICAL_SECTION CreatureTextCriticalSection;
extern CRITICAL_SECTION DrawSkinCriticalSection;
extern CRITICAL_SECTION DrawItemCriticalSection;