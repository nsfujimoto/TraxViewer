using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraxViewer_v2
{
    class Board
    {
        const int BLANK = 0x00;
        const int VERTICAL_W = 0x01;// "+"
        const int HORIZONTAL_W = 0x2;// "+"
        const int UPPER_LEFT_W = 0x04;// "/"
        const int LOWER_RIGHT_W = 0x08;// "/"
        const int UPPER_RIGHT_W = 0x10;// "\"
        const int LOWER_LEFT_W = 0x20;// "\"

        const int RED = 1;
        const int WHITE = 2;
        const int BLANK_COLOR = 0;


        const int UPPER = 0x0001;
        const int LEFT = 0x0002;
        const int RIGHT = 0x0004;
        const int LOWER = 0x0008;

        public int[,] raw_board = new int[512, 512];
        int min_y = 256;
        int min_x = 256;
        int max_x = 256;
        int max_y = 256;

        //右につながる色
        int[] rightcolor =
        {BLANK_COLOR, //BLANK
		RED,	//VETICAL_W
		WHITE, 0,	//HORIZONTAL_W
		RED, 0, 0, 0,	//UPPER_LWFT_W
		WHITE, 0, 0, 0, 0, 0, 0, 0,	//LOWER_RIGHT_W
		WHITE, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, //UPPER_RIGHT_W
		RED}; //LOWER_LEFT_W
              //〃左につながる色
        int[] leftcolor =
        {BLANK_COLOR,
        RED,
        WHITE, 0,
        WHITE, 0, 0, 0,
        RED, 0, 0, 0, 0, 0, 0, 0,
        RED, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        WHITE, };
        //〃上につながる色
        int[] uppercolor =
        {BLANK_COLOR,
        WHITE,
        RED, 0,
        WHITE, 0, 0, 0,
        RED, 0, 0, 0, 0, 0, 0, 0,
        WHITE, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        RED, };
        //〃下につながる色
        int[] lowercolor =
        {BLANK_COLOR,
        WHITE,
        RED, 0,
        RED, 0, 0, 0,
        WHITE, 0, 0, 0, 0, 0, 0, 0,
        RED, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        WHITE, };

        int[] Upper_Connectable = { 0xff, (UPPER_RIGHT_W | UPPER_LEFT_W | HORIZONTAL_W), (LOWER_RIGHT_W | LOWER_LEFT_W | VERTICAL_W) };
        int[] Right_Connectable = { 0xff, (LOWER_RIGHT_W | UPPER_RIGHT_W | VERTICAL_W), (LOWER_LEFT_W | UPPER_LEFT_W | HORIZONTAL_W) };
        int[] Left_Connectable = { 0xff, (LOWER_LEFT_W | UPPER_LEFT_W | VERTICAL_W), (LOWER_RIGHT_W | UPPER_RIGHT_W | HORIZONTAL_W) };
        int[] Lower_Connectable = { 0xff, (LOWER_RIGHT_W | LOWER_LEFT_W | HORIZONTAL_W), (UPPER_RIGHT_W | UPPER_LEFT_W | VERTICAL_W) };


        public int place(int xx, int yy, int tile)
        {
            int x, y;
            int t;
            x = xx + min_x;
            y = yy + min_y;

            t = Right_Connectable[rightcolor[raw_board[x - 1, y]]] &
                Left_Connectable[leftcolor[raw_board[x + 1, y]]] &
                Upper_Connectable[uppercolor[raw_board[x, y + 1]]] &
                Lower_Connectable[lowercolor[raw_board[x, y - 1]]];
            if ((t & tile) == 0) return -1; //配置できない場所なら-1をreturn

            raw_board[x, y] = tile;

            //強制手処理
            if (raw_board[x - 1, y] == BLANK)
            {
                //左強制手処理
                t = Right_Connectable[rightcolor[raw_board[x - 2, y]]] &
                    Left_Connectable[leftcolor[raw_board[x, y]]] &
                    Upper_Connectable[uppercolor[raw_board[x - 1, y + 1]]] &
                    Lower_Connectable[lowercolor[raw_board[x - 1, y - 1]]];
                if ((t & (t - 1)) == 0) force_place(x - 1, y, t);
            }
            if (raw_board[x + 1, y] == BLANK)
            {
                //右強制手処理
                t = Right_Connectable[rightcolor[raw_board[x, y]]] &
                    Left_Connectable[leftcolor[raw_board[x + 2, y]]] &
                    Upper_Connectable[uppercolor[raw_board[x + 1, y + 1]]] &
                    Lower_Connectable[lowercolor[raw_board[x + 1, y - 1]]];
                if ((t & (t - 1)) == 0) force_place(x + 1, y, t);
            }
            if (raw_board[x, y - 1] == BLANK)
            {
                //上強制手処理
                t = Right_Connectable[rightcolor[raw_board[x - 1, y - 1]]] &
                    Left_Connectable[leftcolor[raw_board[x + 1, y - 1]]] &
                    Upper_Connectable[uppercolor[raw_board[x, y]]] &
                    Lower_Connectable[lowercolor[raw_board[x, y - 2]]];
                if ((t & (t - 1)) == 0) force_place(x, y - 1, t);
            }
            if (raw_board[x, y + 1] == BLANK)
            {
                //下強制手処理
                t = Right_Connectable[rightcolor[raw_board[x - 1, y + 1]]] &
                    Left_Connectable[leftcolor[raw_board[x + 1, y + 1]]] &
                    Upper_Connectable[uppercolor[raw_board[x, y + 2]]] &
                    Lower_Connectable[lowercolor[raw_board[x, y]]];
                if ((t & (t - 1)) == 0) force_place(x, y + 1, t);
            }

            if (xx == 0)
            {
                min_x--;
                x--;
            }
            if (xx > max_x)
            {
                max_x++;
            }
            if (yy == 0)
            {
                min_y--;
                y--;
            }
            if(yy > max_y)
            {
                max_y++;
            }

            return 1;
        }
        public void force_place(int x, int y, int tile)
        {
            int t;
            int placeable = BLANK;

            raw_board[x, y] = tile;

            // ４近傍の空きマスを配置可能リストに追加
            if (raw_board[x - 1, y] == BLANK)
            {
                //左強制手処理
                t = Right_Connectable[rightcolor[raw_board[x - 2, y]]] &
                    Left_Connectable[leftcolor[raw_board[x, y]]] &
                    Upper_Connectable[uppercolor[raw_board[x - 1, y + 1]]] &
                    Lower_Connectable[lowercolor[raw_board[x - 1, y - 1]]];
                if ((t & (t - 1)) == 0) force_place(x - 1, y, t);
            }
            if (raw_board[x + 1, y] == BLANK)
            {
                //右強制手処理
                t = Right_Connectable[rightcolor[raw_board[x, y]]] &
                    Left_Connectable[leftcolor[raw_board[x + 2, y]]] &
                    Upper_Connectable[uppercolor[raw_board[x + 1, y + 1]]] &
                    Lower_Connectable[lowercolor[raw_board[x + 1, y - 1]]];
                if ((t & (t - 1)) == 0) force_place(x + 1, y, t);
            }
            if (raw_board[x, y - 1] == BLANK)
            {
                //上強制手処理
                t = Right_Connectable[rightcolor[raw_board[x - 1, y - 1]]] &
                    Left_Connectable[leftcolor[raw_board[x + 1, y - 1]]] &
                    Upper_Connectable[uppercolor[raw_board[x, y]]] &
                    Lower_Connectable[lowercolor[raw_board[x, y - 2]]];
                if ((t & (t - 1)) == 0) force_place(x, y - 1, t);
            }
            if (raw_board[x, y + 1] == BLANK)
            {
                //下強制手処理
                t = Right_Connectable[rightcolor[raw_board[x - 1, y + 1]]] &
                    Left_Connectable[leftcolor[raw_board[x + 1, y + 1]]] &
                    Upper_Connectable[uppercolor[raw_board[x, y + 2]]] &
                    Lower_Connectable[lowercolor[raw_board[x, y]]];
                if ((t & (t - 1)) == 0) force_place(x, y + 1, t);
            }
        }


        public int sn_convert_place(string m)
        {
            int step = 0;
            int pos = 0;
            int x = 0;
            int y = 0;
            char tile = '0';

            while (pos < m.Length)
            {
                char mm = m[pos];
                switch (step)
                {
                    case 0:
                        if (mm == '@') { x = 0; break; }
                        if ('A' <= mm && mm <= 'Z') { x *= 26; x += mm - 'A' + 1; break; }
                        y = mm - '0';
                        step = 1;
                        break;
                    case 1:
                        if ('0' <= mm && mm <= '9') { y *= 10; y += mm - '0'; break; }
                        tile = mm;
                        step = 2;
                        break;
                    case 2:
                        break;
                }
                pos++;
            }
            if (tile == '\\')
            {
                if (place(x, y, UPPER_RIGHT_W) == -1)
                {
                    if (place(x, y, LOWER_LEFT_W) == -1)
                    {
                        return -1;
                    }
                    else return 1;
                }
                else return 1;
            }
            else if (tile == '/')
            {
                if (place(x, y, UPPER_LEFT_W) == -1)
                {
                    if (place(x, y, LOWER_RIGHT_W) == -1)
                    {
                        return -1;
                    }
                    else return 1;
                }
                else return 1;
            }
            else if (tile == '+')
            {
                if (place(x, y, VERTICAL_W) == -1)
                {
                    if (place(x, y, HORIZONTAL_W) == -1)
                    {
                        return -1;
                    }
                    else return 1;
                }
                else return 1;
            }
            else return -1;
        }
    }
}
