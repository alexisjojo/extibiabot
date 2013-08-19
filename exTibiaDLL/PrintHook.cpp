#include "StdAfx.h"
#include <windows.h>
#include <string>
#include <sstream>
#include <list>
#include <assert.h>
#include "PrintHook.h"
#include "Constants.h"
#include "hook.h"
#include "dllmain.h"

std::list<NormalText> DisplayTexts;			//Used for normal text displyaing
std::list<PlayerText> CreatureTexts;		//Used for storing current text to display above creature
std::list<Icon> Icons;
std::list<Skin> Skins;

DWORD OldPrintName = 0;				//Used for restoring PrintText when uninjecting DLL
DWORD OldPrintFPS = 0;				//Used for restoring PrintFPS when uninjecting DLL


_PrintText* PrintText = 0;
_DrawItem* DrawItem = 0;
_DrawSkin *DrawSkin = 0;

bool case_insensitive_compare( std::string a, std::string b)
{ 
	char char1, char2, blank = ' ' ;   
	int len1 = a.length() ; 
	int len2 = b.length() ;  

	if (len1 != len2) return false ; 
	
	for (int i = 0 ; i < len1 ; ++i )
	{ 
		// get a single character from the current position in the string
		char1 = *(a.substr(i,1).data());
		char2 = *(b.substr(i,1).data()); 
		// make lowercase for compare 
		char1 |= blank;
		char2 |= blank; 
		//Test
		if (char1 == char2) continue; 
		return false; 
	} 
	//Everything matched up, return true
	return true; 
} 

void MyPrintNameWork( char* lpText, int nX, int nY)
{
	char* someText;
	DWORD entityID=*(DWORD*)(lpText - 4);
	std::list<PlayerText>::iterator it;
	int x,y,font,r,g,b;

	//Displaying texts
	EnterCriticalSection(&CreatureTextCriticalSection);

	for(it = CreatureTexts.begin(); it != CreatureTexts.end(); ++it) 
	{
			//compare insensitive incase creature name isn't case sensitive (thanks DarkstaR)
		if(entityID == it->CreatureId || (it->CreatureId == 0 && case_insensitive_compare(lpText, it->CreatureName)))
		{
			someText=it->DisplayText;
			x=nX + it->RelativeX;
			y=nY + it->RelativeY;
			font=it->TextFont;
			r=it->cR;
			g=it->cG;
			b=it->cB;
			_asm
			{
				push 0
				push b
				push g
				push r
				push font
				push y
				push x
				push 1
				mov ecx, someText
				call PrintText
				add esp,32
			}
		}
	}
	LeaveCriticalSection(&CreatureTextCriticalSection);
	
}

void MyPrintFpsWork()
{
	char* someText;	

	int x, y, font, r, g, b, width, height, guiID, count, itemID, bSize;

	std::list<NormalText>::iterator ntIT;	

	EnterCriticalSection(&NormalTextCriticalSection);

	for(ntIT = DisplayTexts.begin(); ntIT != DisplayTexts.end(); ++ntIT)
	{
		someText=ntIT->text;
		x=ntIT->x;
		y=ntIT->y;
		font=ntIT->font;
		r=ntIT->r;
		g=ntIT->g;
		b=ntIT->b;
		_asm
		{
			push 0
			push b
			push g
			push r
			push font
			push y
			push x
			push 1
			mov ecx, someText
			call PrintText
			add esp,32
		}
	}

	LeaveCriticalSection(&NormalTextCriticalSection);

	EnterCriticalSection(&DrawSkinCriticalSection);
	std::list<Skin>::iterator sIT;
	for(sIT = Skins.begin(); sIT != Skins.end(); ++sIT)
	{
		x=sIT->X;
		y=sIT->Y;
		width=sIT->Width;
		height=sIT->Height;
		guiID=sIT->GUIId;

		__asm
		{
			push 0
			push 0
			push height
			push width
			push y
			push x
			push 1
			mov ecx, guiID
			call DrawSkin
			add esp, 28
		}
	}
	LeaveCriticalSection(&DrawSkinCriticalSection);
	

	EnterCriticalSection(&DrawItemCriticalSection);
	std::list<Icon>::iterator iIT;
	for(iIT = Icons.begin(); iIT != Icons.end(); ++iIT)
	{
		x=iIT->X;
		y=iIT->Y;
		count=iIT->ItemCount;
		itemID=iIT->ItemId;
		bSize=iIT->BitmapSize;
		font=iIT->TextFont;
		r=iIT->cR;
		g=iIT->cG;
		b=iIT->cB;

		__asm
		{
			push 0
			push 2
			push b
			push g
			push r
			push font
			push bSize
			push bSize
			push y
			push x
			push 0
			push 0
			push 0
			push itemID
			push count
			push 0
			push y
			push x
			push 1
			mov ecx, [bSize]
			call DrawItem
			add esp, 76
		}
	}
	LeaveCriticalSection(&DrawItemCriticalSection);


}

void __declspec(naked) MyPrintName(int nSurface, int nX, int nY, int nFont, int nRed, int nGreen, int nBlue,  int nAlign)
{	
	__asm
	{
		pop edx
		mov ebx, ecx
		call PrintText

		pushad
		pushfd		
		
		//nY
		mov eax, dword ptr ss:[ebp+0xFFFFAF7C]
		add eax, dword ptr ss:[ebp+0xFFFFAF9C]
		push eax
		//nX
		mov eax, dword ptr ss:[ebp+0xFFFFAEF0]
		add eax, dword ptr ss:[ebp+0xFFFFAF90]
		push eax
		//lpText
		push ebx
		call MyPrintNameWork
		add esp, 12

		popfd
		popad
		mov edx,[Consts::ptrPrintName]
		add edx,5
		jmp edx
	}
	
}

void __declspec(naked) MyPrintFps(int nSurface, int nX, int nY, int nFont, int nRed, int nGreen, int nBlue,  int nAlign)
{
	__asm
	{
			pop edx

			mov edx,[Consts::ptrShowFPS]
			cmp [edx],0
			je skipPrintText
			call PrintText

		skipPrintText:

			pushad
			pushfd

			call MyPrintFpsWork

			popfd
			popad

			mov edx, [Consts::ptrPrintFPS]
			add edx, 5
			jmp edx
	}
}

LRESULT WINAPI SubClassProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	if(msg == WM_LBUTTONDOWN)
	{
		POINT pos;
		pos.x=(WORD)lParam;
		pos.y=(WORD)(lParam>>16);
		
		
		EnterCriticalSection(&DrawItemCriticalSection);

		std::list<Icon>::iterator iIT;
		for(iIT = Icons.begin(); iIT != Icons.end(); ++iIT)	
		{
			if(pos.x >=iIT->X && pos.x <=iIT->X+iIT->BitmapSize)
			{
				if(pos.y >=iIT->Y && pos.y <=iIT->Y+iIT->BitmapSize)
				{
					MessageBox(NULL, L"You clicked an item", L"exTibia", MB_OK);				
				}
			}
		}
		LeaveCriticalSection(&DrawItemCriticalSection);
	}


	return fUnicode ? CallWindowProcW(oldWndProc, hwnd, msg, wParam, lParam) : 
					CallWindowProcA(oldWndProc, hwnd, msg, wParam, lParam);
}

void PrintHook::AddPrintText(int posx, int posy, int r, int g, int b, int font, std::string textName, std::string textToPrint)
{
	NormalText NewText;
	NewText.b = r;
	NewText.g = g;
	NewText.r = b;
	NewText.x = posx;
	NewText.y = posy;
	NewText.font = font;

	NewText.TextName = new char[textName.size() + 1];
	NewText.text = new char[textToPrint.size() + 1];

	memcpy(NewText.TextName, textName.c_str(), textName.size() + 1);
	memcpy(NewText.text, textToPrint.c_str(), textToPrint.size() + 1);

	std::list<NormalText>::iterator ntIT;

	EnterCriticalSection(&NormalTextCriticalSection);
	BOOL found = FALSE;
	for(ntIT = DisplayTexts.begin(); ntIT != DisplayTexts.end(); ++ntIT)
	{
		if (strcmp(ntIT->TextName,NewText.TextName) == 0)
		{
			ntIT->text = NewText.text;
			found = TRUE;
		}
	}
	if (found)
	{
		LeaveCriticalSection(&NormalTextCriticalSection);
		return;
	}

	DisplayTexts.push_back(NewText);
	LeaveCriticalSection(&NormalTextCriticalSection);
}

void PrintHook::AddPrintItem(int iconId, int posx, int posy, int size, int itemId, int itemCount, int fontType, int r, int g, int b)
{
	Icon icon;
    icon.IconId=iconId;
    icon.X=posx;;
    icon.Y=posy;
    icon.BitmapSize=size;
    icon.ItemId=itemId;
    icon.ItemCount=itemCount;
    icon.TextFont=fontType;
    icon.cR=r;
    icon.cG=g;
    icon.cB=b;



	/*
	std::list<Item>::iterator ntIT;

	EnterCriticalSection(&NormalTextCriticalSection);
	BOOL found = FALSE;
	for(ntIT = Items.begin(); ntIT != Items.end(); ++ntIT)
	{
		if (strcmp(ntIT->TextName,NewText.TextName) == 0)
		{
			ntIT->text = NewText.text;
			found = TRUE;
		}
	}
	if (found)
	{
		LeaveCriticalSection(&NormalTextCriticalSection);
		return;
	}

	DisplayTexts.push_back(NewText);
	LeaveCriticalSection(&NormalTextCriticalSection);


	*/










        
    EnterCriticalSection(&DrawItemCriticalSection);
    Icons.push_back(icon);
    LeaveCriticalSection(&DrawItemCriticalSection);
}

void PrintHook::RemovePrintText(std::string textName)
{
	std::list<NormalText>::iterator ntIT;
	EnterCriticalSection(&NormalTextCriticalSection);

	for(ntIT = DisplayTexts.begin(); ntIT != DisplayTexts.end(); ++ntIT)
	{
		std::string str(ntIT->TextName);
		if (str.compare(textName) == 0)
		{
			delete [] ntIT->TextName;
			delete [] ntIT->text;

			DisplayTexts.erase(ntIT);
			return;
		}		
	}

	LeaveCriticalSection(&NormalTextCriticalSection);
}

void PrintHook:: ClearPrintText()
{
	std::list<NormalText>::iterator ntIT;
	EnterCriticalSection(&NormalTextCriticalSection);

	for(ntIT = DisplayTexts.begin(); ntIT != DisplayTexts.end(); ++ntIT)
	{
		delete [] ntIT->text;
		delete [] ntIT->TextName;
	}

	DisplayTexts.clear();
	LeaveCriticalSection(&NormalTextCriticalSection);
}

void PrintHook:: ClearPrintItem()
{
	EnterCriticalSection(&DrawItemCriticalSection);
	Icons.clear();
    LeaveCriticalSection(&DrawItemCriticalSection);
}

PrintHook::PrintHook(void)
{
	AddPrintText(10,20,123,145,123,1,"text1", "I, I cant get these memories out of my mind, And some kind of madness has started to evolve.");
	AddPrintText(10,40,249,255,255,2,"text2", "I, I tried so hard to let you go, But some kind of madness is swallowing me whole, yeah");
	AddPrintItem(1, 200, 200, 32, 3393, 50, 2, 255, 0, 0);
}

PrintHook::~PrintHook(void)
{

}
