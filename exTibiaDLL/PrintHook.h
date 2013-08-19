#pragma once
class PrintHook
{
public:
	PrintHook(void);
	~PrintHook(void);

	

	static void PrintHook::AddPrintText(int posx, int posy, int r, int g, int b, int font, std::string textName, std::string textToPrint);
	static void PrintHook::AddPrintItem(int iconId, int posx, int posy, int size, int itemId, int itemCount, int fontType, int r, int g, int b);
	static void PrintHook::RemovePrintText(std::string textName);
	static void PrintHook::RemovePrintItem(std::string itemName);
	
	static void PrintHook::ClearPrintText();
	static void PrintHook::ClearPrintItem();
private:

};

LRESULT WINAPI SubClassProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

extern DWORD OldPrintName;
extern DWORD OldPrintFPS;

typedef void _PrintText(int nSurface, int nX, int nY, int nFont, int nRed, int nGreen, int nBlue, int nAlign);
extern _PrintText* PrintText;

typedef void _DrawItem(int surface, int x, int y, int itemData1, int itemData2, int itemId, int edgeR, int edgeG, int edgeB ,int clipX,int clipY, int clipW, int clipH,int textFont,int textRed,int textGreen,int textBlue,int textAlign,int textForce);
extern _DrawItem* DrawItem;

typedef void _DrawSkin(int surface, int x, int y, int width, int height, int dX, int dY);
extern _DrawSkin *DrawSkin;

void MyPrintName(int nSurface, int nX, int nY, int nFont, int nRed, int nGreen, int nBlue, int nAlign);
void MyPrintFps(int nSurface, int nX, int nY, int nFont, int nRed, int nGreen, int nBlue, int nAlign);