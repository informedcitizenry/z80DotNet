﻿//-----------------------------------------------------------------------------
// Copyright (c) 2017 informedcitizenry <informedcitizenry@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using DotNetAsm;

namespace z80DotNet
{
    /// <summary>
    /// A class that assembles Z80 assembly code.
    /// </summary>
    public partial class z80Asm : AssemblerBase, ILineAssembler
    {
        #region Members

        private Opcode[] _bitsOpcodes = 
        {
            new Opcode(){ Extension = null, DisasmFormat = "rlc b",             Size = 2 }, // cb 00
            new Opcode(){ Extension = null, DisasmFormat = "rlc c",             Size = 2 }, // cb 01
            new Opcode(){ Extension = null, DisasmFormat = "rlc d",             Size = 2 }, // cb 02
            new Opcode(){ Extension = null, DisasmFormat = "rlc e",             Size = 2 }, // cb 03
            new Opcode(){ Extension = null, DisasmFormat = "rlc h",             Size = 2 }, // cb 04
            new Opcode(){ Extension = null, DisasmFormat = "rlc l",             Size = 2 }, // cb 05
            new Opcode(){ Extension = null, DisasmFormat = "rlc (hl)",          Size = 2 }, // cb 06
            new Opcode(){ Extension = null, DisasmFormat = "rlc a",             Size = 2 }, // cb 07
            new Opcode(){ Extension = null, DisasmFormat = "rrc b",             Size = 2 }, // cb 08
            new Opcode(){ Extension = null, DisasmFormat = "rrc c",             Size = 2 }, // cb 09
            new Opcode(){ Extension = null, DisasmFormat = "rrc d",             Size = 2 }, // cb 0a
            new Opcode(){ Extension = null, DisasmFormat = "rrc e",             Size = 2 }, // cb 0b
            new Opcode(){ Extension = null, DisasmFormat = "rrc h",             Size = 2 }, // cb 0c
            new Opcode(){ Extension = null, DisasmFormat = "rrc l",             Size = 2 }, // cb 0d
            new Opcode(){ Extension = null, DisasmFormat = "rrc (hl)",          Size = 2 }, // cb 0e
            new Opcode(){ Extension = null, DisasmFormat = "rrc a",             Size = 2 }, // cb 0f
            new Opcode(){ Extension = null, DisasmFormat = "rl b",              Size = 2 }, // cb 10
            new Opcode(){ Extension = null, DisasmFormat = "rl c",              Size = 2 }, // cb 11
            new Opcode(){ Extension = null, DisasmFormat = "rl d",              Size = 2 }, // cb 12
            new Opcode(){ Extension = null, DisasmFormat = "rl e",              Size = 2 }, // cb 13
            new Opcode(){ Extension = null, DisasmFormat = "rl h",              Size = 2 }, // cb 14
            new Opcode(){ Extension = null, DisasmFormat = "rl l",              Size = 2 }, // cb 15
            new Opcode(){ Extension = null, DisasmFormat = "rl (hl)",           Size = 2 }, // cb 16
            new Opcode(){ Extension = null, DisasmFormat = "rl a",              Size = 2 }, // cb 17
            new Opcode(){ Extension = null, DisasmFormat = "rr b",              Size = 2 }, // cb 18
            new Opcode(){ Extension = null, DisasmFormat = "rr c",              Size = 2 }, // cb 19
            new Opcode(){ Extension = null, DisasmFormat = "rr d",              Size = 2 }, // cb 1a
            new Opcode(){ Extension = null, DisasmFormat = "rr e",              Size = 2 }, // cb 1b
            new Opcode(){ Extension = null, DisasmFormat = "rr h",              Size = 2 }, // cb 1c
            new Opcode(){ Extension = null, DisasmFormat = "rr l",              Size = 2 }, // cb 1d
            new Opcode(){ Extension = null, DisasmFormat = "rr (hl)",           Size = 2 }, // cb 1e
            new Opcode(){ Extension = null, DisasmFormat = "rr a",              Size = 2 }, // cb 1f
            new Opcode(){ Extension = null, DisasmFormat = "sla b",             Size = 2 }, // cb 20
            new Opcode(){ Extension = null, DisasmFormat = "sla c",             Size = 2 }, // cb 21
            new Opcode(){ Extension = null, DisasmFormat = "sla d",             Size = 2 }, // cb 22
            new Opcode(){ Extension = null, DisasmFormat = "sla e",             Size = 2 }, // cb 23
            new Opcode(){ Extension = null, DisasmFormat = "sla h",             Size = 2 }, // cb 24
            new Opcode(){ Extension = null, DisasmFormat = "sla l",             Size = 2 }, // cb 25
            new Opcode(){ Extension = null, DisasmFormat = "sla (hl)",          Size = 2 }, // cb 26
            new Opcode(){ Extension = null, DisasmFormat = "sla a",             Size = 2 }, // cb 27
            new Opcode(){ Extension = null, DisasmFormat = "sra b",             Size = 2 }, // cb 28
            new Opcode(){ Extension = null, DisasmFormat = "sra c",             Size = 2 }, // cb 29
            new Opcode(){ Extension = null, DisasmFormat = "sra d",             Size = 2 }, // cb 2a
            new Opcode(){ Extension = null, DisasmFormat = "sra e",             Size = 2 }, // cb 2b
            new Opcode(){ Extension = null, DisasmFormat = "sra h",             Size = 2 }, // cb 2c
            new Opcode(){ Extension = null, DisasmFormat = "sra l",             Size = 2 }, // cb 2d
            new Opcode(){ Extension = null, DisasmFormat = "sra (hl)",          Size = 2 }, // cb 2e
            new Opcode(){ Extension = null, DisasmFormat = "sra a",             Size = 2 }, // cb 2f
            new Opcode(){ Extension = null, DisasmFormat = "sll b",             Size = 2 }, // cb 30
            new Opcode(){ Extension = null, DisasmFormat = "sll c",             Size = 2 }, // cb 31
            new Opcode(){ Extension = null, DisasmFormat = "sll d",             Size = 2 }, // cb 32
            new Opcode(){ Extension = null, DisasmFormat = "sll e",             Size = 2 }, // cb 33
            new Opcode(){ Extension = null, DisasmFormat = "sll h",             Size = 2 }, // cb 34
            new Opcode(){ Extension = null, DisasmFormat = "sll l",             Size = 2 }, // cb 35
            new Opcode(){ Extension = null, DisasmFormat = "sll (hl)",          Size = 2 }, // cb 36
            new Opcode(){ Extension = null, DisasmFormat = "sll a",             Size = 2 }, // cb 37
            new Opcode(){ Extension = null, DisasmFormat = "srl b",             Size = 2 }, // cb 38
            new Opcode(){ Extension = null, DisasmFormat = "srl c",             Size = 2 }, // cb 39
            new Opcode(){ Extension = null, DisasmFormat = "srl d",             Size = 2 }, // cb 3a
            new Opcode(){ Extension = null, DisasmFormat = "srl e",             Size = 2 }, // cb 3b
            new Opcode(){ Extension = null, DisasmFormat = "srl h",             Size = 2 }, // cb 3c
            new Opcode(){ Extension = null, DisasmFormat = "srl l",             Size = 2 }, // cb 3d
            new Opcode(){ Extension = null, DisasmFormat = "srl (hl)",          Size = 2 }, // cb 3e
            new Opcode(){ Extension = null, DisasmFormat = "srl a",             Size = 2 }, // cb 3f
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,b",           Size = 2 }, // cb 40
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,c",           Size = 2 }, // cb 41
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,d",           Size = 2 }, // cb 42
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,e",           Size = 2 }, // cb 43
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,h",           Size = 2 }, // cb 44
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,l",           Size = 2 }, // cb 45
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(hl)",        Size = 2 }, // cb 46
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,a",           Size = 2 }, // cb 47
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,b",           Size = 2 }, // cb 48
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,c",           Size = 2 }, // cb 49
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,d",           Size = 2 }, // cb 4a
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,e",           Size = 2 }, // cb 4b
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,h",           Size = 2 }, // cb 4c
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,l",           Size = 2 }, // cb 4d
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(hl)",        Size = 2 }, // cb 4e
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,a",           Size = 2 }, // cb 4f
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,b",           Size = 2 }, // cb 50
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,c",           Size = 2 }, // cb 51
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,d",           Size = 2 }, // cb 52
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,e",           Size = 2 }, // cb 53
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,h",           Size = 2 }, // cb 54
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,l",           Size = 2 }, // cb 55
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(hl)",        Size = 2 }, // cb 56
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,a",           Size = 2 }, // cb 57
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,b",           Size = 2 }, // cb 58
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,c",           Size = 2 }, // cb 59
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,d",           Size = 2 }, // cb 5a
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,e",           Size = 2 }, // cb 5b
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,h",           Size = 2 }, // cb 5c
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,l",           Size = 2 }, // cb 5d
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(hl)",        Size = 2 }, // cb 5e
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,a",           Size = 2 }, // cb 5f
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,b",           Size = 2 }, // cb 60
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,c",           Size = 2 }, // cb 61
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,d",           Size = 2 }, // cb 62
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,e",           Size = 2 }, // cb 63
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,h",           Size = 2 }, // cb 64
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,l",           Size = 2 }, // cb 65
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(hl)",        Size = 2 }, // cb 66
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,a",           Size = 2 }, // cb 67
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,b",           Size = 2 }, // cb 68
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,c",           Size = 2 }, // cb 69
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,d",           Size = 2 }, // cb 6a
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,e",           Size = 2 }, // cb 6b
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,h",           Size = 2 }, // cb 6c
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,l",           Size = 2 }, // cb 6d
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(hl)",        Size = 2 }, // cb 6e
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,a",           Size = 2 }, // cb 6f
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,b",           Size = 2 }, // cb 70
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,c",           Size = 2 }, // cb 71
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,d",           Size = 2 }, // cb 72
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,e",           Size = 2 }, // cb 73
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,h",           Size = 2 }, // cb 74
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,l",           Size = 2 }, // cb 75
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(hl)",        Size = 2 }, // cb 76
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,a",           Size = 2 }, // cb 77
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,b",           Size = 2 }, // cb 78
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,c",           Size = 2 }, // cb 79
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,d",           Size = 2 }, // cb 7a
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,e",           Size = 2 }, // cb 7b
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,h",           Size = 2 }, // cb 7c
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,l",           Size = 2 }, // cb 7d
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(hl)",        Size = 2 }, // cb 7e
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,a",           Size = 2 }, // cb 7f
            new Opcode(){ Extension = null, DisasmFormat = "res 0,b",           Size = 2 }, // cb 80
            new Opcode(){ Extension = null, DisasmFormat = "res 0,c",           Size = 2 }, // cb 81
            new Opcode(){ Extension = null, DisasmFormat = "res 0,d",           Size = 2 }, // cb 82
            new Opcode(){ Extension = null, DisasmFormat = "res 0,e",           Size = 2 }, // cb 83
            new Opcode(){ Extension = null, DisasmFormat = "res 0,h",           Size = 2 }, // cb 84
            new Opcode(){ Extension = null, DisasmFormat = "res 0,l",           Size = 2 }, // cb 85
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(hl)",        Size = 2 }, // cb 86
            new Opcode(){ Extension = null, DisasmFormat = "res 0,a",           Size = 2 }, // cb 87
            new Opcode(){ Extension = null, DisasmFormat = "res 1,b",           Size = 2 }, // cb 88
            new Opcode(){ Extension = null, DisasmFormat = "res 1,c",           Size = 2 }, // cb 89
            new Opcode(){ Extension = null, DisasmFormat = "res 1,d",           Size = 2 }, // cb 8a
            new Opcode(){ Extension = null, DisasmFormat = "res 1,e",           Size = 2 }, // cb 8b
            new Opcode(){ Extension = null, DisasmFormat = "res 1,h",           Size = 2 }, // cb 8c
            new Opcode(){ Extension = null, DisasmFormat = "res 1,l",           Size = 2 }, // cb 8d
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(hl)",        Size = 2 }, // cb 8e
            new Opcode(){ Extension = null, DisasmFormat = "res 1,a",           Size = 2 }, // cb 8f
            new Opcode(){ Extension = null, DisasmFormat = "res 2,b",           Size = 2 }, // cb 90
            new Opcode(){ Extension = null, DisasmFormat = "res 2,c",           Size = 2 }, // cb 91
            new Opcode(){ Extension = null, DisasmFormat = "res 2,d",           Size = 2 }, // cb 92
            new Opcode(){ Extension = null, DisasmFormat = "res 2,e",           Size = 2 }, // cb 93
            new Opcode(){ Extension = null, DisasmFormat = "res 2,h",           Size = 2 }, // cb 94
            new Opcode(){ Extension = null, DisasmFormat = "res 2,l",           Size = 2 }, // cb 95
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(hl)",        Size = 2 }, // cb 96
            new Opcode(){ Extension = null, DisasmFormat = "res 2,a",           Size = 2 }, // cb 97
            new Opcode(){ Extension = null, DisasmFormat = "res 3,b",           Size = 2 }, // cb 98
            new Opcode(){ Extension = null, DisasmFormat = "res 3,c",           Size = 2 }, // cb 99
            new Opcode(){ Extension = null, DisasmFormat = "res 3,d",           Size = 2 }, // cb 9a
            new Opcode(){ Extension = null, DisasmFormat = "res 3,e",           Size = 2 }, // cb 9b
            new Opcode(){ Extension = null, DisasmFormat = "res 3,h",           Size = 2 }, // cb 9c
            new Opcode(){ Extension = null, DisasmFormat = "res 3,l",           Size = 2 }, // cb 9d
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(hl)",        Size = 2 }, // cb 9e
            new Opcode(){ Extension = null, DisasmFormat = "res 3,a",           Size = 2 }, // cb 9f
            new Opcode(){ Extension = null, DisasmFormat = "res 4,b",           Size = 2 }, // cb a0
            new Opcode(){ Extension = null, DisasmFormat = "res 4,c",           Size = 2 }, // cb a1
            new Opcode(){ Extension = null, DisasmFormat = "res 4,d",           Size = 2 }, // cb a2
            new Opcode(){ Extension = null, DisasmFormat = "res 4,e",           Size = 2 }, // cb a3
            new Opcode(){ Extension = null, DisasmFormat = "res 4,h",           Size = 2 }, // cb a4
            new Opcode(){ Extension = null, DisasmFormat = "res 4,l",           Size = 2 }, // cb a5
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(hl)",        Size = 2 }, // cb a6
            new Opcode(){ Extension = null, DisasmFormat = "res 4,a",           Size = 2 }, // cb a7
            new Opcode(){ Extension = null, DisasmFormat = "res 5,b",           Size = 2 }, // cb a8
            new Opcode(){ Extension = null, DisasmFormat = "res 5,c",           Size = 2 }, // cb a9
            new Opcode(){ Extension = null, DisasmFormat = "res 5,d",           Size = 2 }, // cb aa
            new Opcode(){ Extension = null, DisasmFormat = "res 5,e",           Size = 2 }, // cb ab
            new Opcode(){ Extension = null, DisasmFormat = "res 5,h",           Size = 2 }, // cb ac
            new Opcode(){ Extension = null, DisasmFormat = "res 5,l",           Size = 2 }, // cb ad
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(hl)",        Size = 2 }, // cb ae
            new Opcode(){ Extension = null, DisasmFormat = "res 5,a",           Size = 2 }, // cb af
            new Opcode(){ Extension = null, DisasmFormat = "res 6,b",           Size = 2 }, // cb b0
            new Opcode(){ Extension = null, DisasmFormat = "res 6,c",           Size = 2 }, // cb b1
            new Opcode(){ Extension = null, DisasmFormat = "res 6,d",           Size = 2 }, // cb b2
            new Opcode(){ Extension = null, DisasmFormat = "res 6,e",           Size = 2 }, // cb b3
            new Opcode(){ Extension = null, DisasmFormat = "res 6,h",           Size = 2 }, // cb b4
            new Opcode(){ Extension = null, DisasmFormat = "res 6,l",           Size = 2 }, // cb b5
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(hl)",        Size = 2 }, // cb b6
            new Opcode(){ Extension = null, DisasmFormat = "res 6,a",           Size = 2 }, // cb b7
            new Opcode(){ Extension = null, DisasmFormat = "res 7,b",           Size = 2 }, // cb b8
            new Opcode(){ Extension = null, DisasmFormat = "res 7,c",           Size = 2 }, // cb b9
            new Opcode(){ Extension = null, DisasmFormat = "res 7,d",           Size = 2 }, // cb ba
            new Opcode(){ Extension = null, DisasmFormat = "res 7,e",           Size = 2 }, // cb bb
            new Opcode(){ Extension = null, DisasmFormat = "res 7,h",           Size = 2 }, // cb bc
            new Opcode(){ Extension = null, DisasmFormat = "res 7,l",           Size = 2 }, // cb bd
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(hl)",        Size = 2 }, // cb be
            new Opcode(){ Extension = null, DisasmFormat = "res 7,a",           Size = 2 }, // cb bf
            new Opcode(){ Extension = null, DisasmFormat = "set 0,b",           Size = 2 }, // cb c0
            new Opcode(){ Extension = null, DisasmFormat = "set 0,c",           Size = 2 }, // cb c1
            new Opcode(){ Extension = null, DisasmFormat = "set 0,d",           Size = 2 }, // cb c2
            new Opcode(){ Extension = null, DisasmFormat = "set 0,e",           Size = 2 }, // cb c3
            new Opcode(){ Extension = null, DisasmFormat = "set 0,h",           Size = 2 }, // cb c4
            new Opcode(){ Extension = null, DisasmFormat = "set 0,l",           Size = 2 }, // cb c5
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(hl)",        Size = 2 }, // cb c6
            new Opcode(){ Extension = null, DisasmFormat = "set 0,a",           Size = 2 }, // cb c7
            new Opcode(){ Extension = null, DisasmFormat = "set 1,b",           Size = 2 }, // cb c8
            new Opcode(){ Extension = null, DisasmFormat = "set 1,c",           Size = 2 }, // cb c9
            new Opcode(){ Extension = null, DisasmFormat = "set 1,d",           Size = 2 }, // cb ca
            new Opcode(){ Extension = null, DisasmFormat = "set 1,e",           Size = 2 }, // cb cb
            new Opcode(){ Extension = null, DisasmFormat = "set 1,h",           Size = 2 }, // cb cc
            new Opcode(){ Extension = null, DisasmFormat = "set 1,l",           Size = 2 }, // cb cd
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(hl)",        Size = 2 }, // cb ce
            new Opcode(){ Extension = null, DisasmFormat = "set 1,a",           Size = 2 }, // cb cf
            new Opcode(){ Extension = null, DisasmFormat = "set 2,b",           Size = 2 }, // cb d0
            new Opcode(){ Extension = null, DisasmFormat = "set 2,c",           Size = 2 }, // cb d1
            new Opcode(){ Extension = null, DisasmFormat = "set 2,d",           Size = 2 }, // cb d2
            new Opcode(){ Extension = null, DisasmFormat = "set 2,e",           Size = 2 }, // cb d3
            new Opcode(){ Extension = null, DisasmFormat = "set 2,h",           Size = 2 }, // cb d4
            new Opcode(){ Extension = null, DisasmFormat = "set 2,l",           Size = 2 }, // cb d5
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(hl)",        Size = 2 }, // cb d6
            new Opcode(){ Extension = null, DisasmFormat = "set 2,a",           Size = 2 }, // cb d7
            new Opcode(){ Extension = null, DisasmFormat = "set 3,b",           Size = 2 }, // cb d8
            new Opcode(){ Extension = null, DisasmFormat = "set 3,c",           Size = 2 }, // cb d9
            new Opcode(){ Extension = null, DisasmFormat = "set 3,d",           Size = 2 }, // cb da
            new Opcode(){ Extension = null, DisasmFormat = "set 3,e",           Size = 2 }, // cb db
            new Opcode(){ Extension = null, DisasmFormat = "set 3,h",           Size = 2 }, // cb dc
            new Opcode(){ Extension = null, DisasmFormat = "set 3,l",           Size = 2 }, // cb dd
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(hl)",        Size = 2 }, // cb de
            new Opcode(){ Extension = null, DisasmFormat = "set 3,a",           Size = 2 }, // cb df
            new Opcode(){ Extension = null, DisasmFormat = "set 4,b",           Size = 2 }, // cb e0
            new Opcode(){ Extension = null, DisasmFormat = "set 4,c",           Size = 2 }, // cb e1
            new Opcode(){ Extension = null, DisasmFormat = "set 4,d",           Size = 2 }, // cb e2
            new Opcode(){ Extension = null, DisasmFormat = "set 4,e",           Size = 2 }, // cb e3
            new Opcode(){ Extension = null, DisasmFormat = "set 4,h",           Size = 2 }, // cb e4
            new Opcode(){ Extension = null, DisasmFormat = "set 4,l",           Size = 2 }, // cb e5
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(hl)",        Size = 2 }, // cb e6
            new Opcode(){ Extension = null, DisasmFormat = "set 4,a",           Size = 2 }, // cb e7
            new Opcode(){ Extension = null, DisasmFormat = "set 5,b",           Size = 2 }, // cb e8
            new Opcode(){ Extension = null, DisasmFormat = "set 5,c",           Size = 2 }, // cb e9
            new Opcode(){ Extension = null, DisasmFormat = "set 5,d",           Size = 2 }, // cb ea
            new Opcode(){ Extension = null, DisasmFormat = "set 5,e",           Size = 2 }, // cb eb
            new Opcode(){ Extension = null, DisasmFormat = "set 5,h",           Size = 2 }, // cb ec
            new Opcode(){ Extension = null, DisasmFormat = "set 5,l",           Size = 2 }, // cb ed
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(hl)",        Size = 2 }, // cb ee
            new Opcode(){ Extension = null, DisasmFormat = "set 5,a",           Size = 2 }, // cb ef
            new Opcode(){ Extension = null, DisasmFormat = "set 6,b",           Size = 2 }, // cb f0
            new Opcode(){ Extension = null, DisasmFormat = "set 6,c",           Size = 2 }, // cb f1
            new Opcode(){ Extension = null, DisasmFormat = "set 6,d",           Size = 2 }, // cb f2
            new Opcode(){ Extension = null, DisasmFormat = "set 6,e",           Size = 2 }, // cb f3
            new Opcode(){ Extension = null, DisasmFormat = "set 6,h",           Size = 2 }, // cb f4
            new Opcode(){ Extension = null, DisasmFormat = "set 6,l",           Size = 2 }, // cb f5
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(hl)",        Size = 2 }, // cb f6
            new Opcode(){ Extension = null, DisasmFormat = "set 6,a",           Size = 2 }, // cb f7
            new Opcode(){ Extension = null, DisasmFormat = "set 7,b",           Size = 2 }, // cb f8
            new Opcode(){ Extension = null, DisasmFormat = "set 7,c",           Size = 2 }, // cb f9
            new Opcode(){ Extension = null, DisasmFormat = "set 7,d",           Size = 2 }, // cb fa
            new Opcode(){ Extension = null, DisasmFormat = "set 7,e",           Size = 2 }, // cb fb
            new Opcode(){ Extension = null, DisasmFormat = "set 7,h",           Size = 2 }, // cb fc
            new Opcode(){ Extension = null, DisasmFormat = "set 7,l",           Size = 2 }, // cb fd
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(hl)",        Size = 2 }, // cb fe
            new Opcode(){ Extension = null, DisasmFormat = "set 7,a",           Size = 2 }  // cb ff
        };

        private Opcode[] _exOpcodes = 
        {
            null                                                                          , // ed 00
            null                                                                          , // ed 01
            null                                                                          , // ed 02
            null                                                                          , // ed 03
            null                                                                          , // ed 04
            null                                                                          , // ed 05
            null                                                                          , // ed 06
            null                                                                          , // ed 07
            null                                                                          , // ed 08
            null                                                                          , // ed 09
            null                                                                          , // ed 0a
            null                                                                          , // ed 0b
            null                                                                          , // ed 0c
            null                                                                          , // ed 0d
            null                                                                          , // ed 0e
            null                                                                          , // ed 0f
            null                                                                          , // ed 10
            null                                                                          , // ed 11
            null                                                                          , // ed 12
            null                                                                          , // ed 13
            null                                                                          , // ed 14
            null                                                                          , // ed 15
            null                                                                          , // ed 16
            null                                                                          , // ed 17
            null                                                                          , // ed 18
            null                                                                          , // ed 19
            null                                                                          , // ed 1a
            null                                                                          , // ed 1b
            null                                                                          , // ed 1c
            null                                                                          , // ed 1d
            null                                                                          , // ed 1e
            null                                                                          , // ed 1f
            null                                                                          , // ed 20
            null                                                                          , // ed 21
            null                                                                          , // ed 22
            null                                                                          , // ed 23
            null                                                                          , // ed 24
            null                                                                          , // ed 25
            null                                                                          , // ed 26
            null                                                                          , // ed 27
            null                                                                          , // ed 28
            null                                                                          , // ed 29
            null                                                                          , // ed 2a
            null                                                                          , // ed 2b
            null                                                                          , // ed 2c
            null                                                                          , // ed 2d
            null                                                                          , // ed 2e
            null                                                                          , // ed 2f
            null                                                                          , // ed 30
            null                                                                          , // ed 31
            null                                                                          , // ed 32
            null                                                                          , // ed 33
            null                                                                          , // ed 34
            null                                                                          , // ed 35
            null                                                                          , // ed 36
            null                                                                          , // ed 37
            null                                                                          , // ed 38
            null                                                                          , // ed 39
            null                                                                          , // ed 3a
            null                                                                          , // ed 3b
            null                                                                          , // ed 3c
            null                                                                          , // ed 3d
            null                                                                          , // ed 3e
            null                                                                          , // ed 3f
            new Opcode(){ Extension = null, DisasmFormat = "in b,(c)",          Size = 2 }, // ed 40
            new Opcode(){ Extension = null, DisasmFormat = "out (c),b",         Size = 2 }, // ed 41
            new Opcode(){ Extension = null, DisasmFormat = "sbc hl,bc",         Size = 2 }, // ed 42
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),bc",   Size = 4 }, // ed 43
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 44
            new Opcode(){ Extension = null, DisasmFormat = "retn",              Size = 2 }, // ed 45
            new Opcode(){ Extension = null, DisasmFormat = "im 0",              Size = 2 }, // ed 46
            new Opcode(){ Extension = null, DisasmFormat = "ld i,a",            Size = 2 }, // ed 47
            new Opcode(){ Extension = null, DisasmFormat = "in c,(c)",          Size = 2 }, // ed 48
            new Opcode(){ Extension = null, DisasmFormat = "out (c),c",         Size = 2 }, // ed 49
            new Opcode(){ Extension = null, DisasmFormat = "adc hl,bc",         Size = 2 }, // ed 4a
            new Opcode(){ Extension = null, DisasmFormat = "ld bc,(${0:x4})",   Size = 4 }, // ed 4b
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 4c
            new Opcode(){ Extension = null, DisasmFormat = "reti",              Size = 2 }, // ed 4d
            null,                                                                           // ed 4e
            new Opcode(){ Extension = null, DisasmFormat = "ld r,a",            Size = 2 }, // ed 4f
            new Opcode(){ Extension = null, DisasmFormat = "in d,(c)",          Size = 2 }, // ed 50
            new Opcode(){ Extension = null, DisasmFormat = "out (c),d",         Size = 2 }, // ed 51
            new Opcode(){ Extension = null, DisasmFormat = "sbc hl,de",         Size = 2 }, // ed 52
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),de",   Size = 4 }, // ed 53
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 54
            new Opcode(){ Extension = null, DisasmFormat = "retn",              Size = 2 }, // ed 55
            new Opcode(){ Extension = null, DisasmFormat = "im 1",              Size = 2 }, // ed 56
            new Opcode(){ Extension = null, DisasmFormat = "ld a,i",            Size = 2 }, // ed 57
            new Opcode(){ Extension = null, DisasmFormat = "in e,(c)",          Size = 2 }, // ed 58
            new Opcode(){ Extension = null, DisasmFormat = "out (c),e",         Size = 2 }, // ed 59
            new Opcode(){ Extension = null, DisasmFormat = "adc hl,de",         Size = 2 }, // ed 5a
            new Opcode(){ Extension = null, DisasmFormat = "ld de,(${0:x4})",   Size = 4 }, // ed 5b
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 5c
            new Opcode(){ Extension = null, DisasmFormat = "retn",              Size = 2 }, // ed 5d
            new Opcode(){ Extension = null, DisasmFormat = "im 2",              Size = 2 }, // ed 5e
            new Opcode(){ Extension = null, DisasmFormat = "ld a,r",            Size = 2 }, // ed 5f
            new Opcode(){ Extension = null, DisasmFormat = "in h,(c)",          Size = 2 }, // ed 60
            new Opcode(){ Extension = null, DisasmFormat = "out (c),h",         Size = 2 }, // ed 61
            new Opcode(){ Extension = null, DisasmFormat = "sbc hl,hl",         Size = 2 }, // ed 62
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),hl",   Size = 4 }, // ed 63
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 64
            new Opcode(){ Extension = null, DisasmFormat = "retn",              Size = 2 }, // ed 65
            new Opcode(){ Extension = null, DisasmFormat = "im 0",              Size = 2 }, // ed 66
            new Opcode(){ Extension = null, DisasmFormat = "rrd",               Size = 2 }, // ed 67
            new Opcode(){ Extension = null, DisasmFormat = "in l,(c)",          Size = 2 }, // ed 68
            new Opcode(){ Extension = null, DisasmFormat = "out (c),l",         Size = 2 }, // ed 69
            new Opcode(){ Extension = null, DisasmFormat = "adc hl,hl",         Size = 2 }, // ed 6a
            new Opcode(){ Extension = null, DisasmFormat = "ld hl,(${0:x4})",   Size = 4 }, // ed 6b
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 6c
            new Opcode(){ Extension = null, DisasmFormat = "retn",              Size = 2 }, // ed 6d
            null,                                                                           // ed 6e
            new Opcode(){ Extension = null, DisasmFormat = "rld",               Size = 2 }, // ed 6f
            new Opcode(){ Extension = null, DisasmFormat = "in (c)",            Size = 2 }, // ed 70
            new Opcode(){ Extension = null, DisasmFormat = "out (c),0",         Size = 2 }, // ed 71
            new Opcode(){ Extension = null, DisasmFormat = "sbc hl,sp",         Size = 2 }, // ed 72
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),sp",   Size = 4 }, // ed 73
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 74
            new Opcode(){ Extension = null, DisasmFormat = "retn",              Size = 2 }, // ed 75
            new Opcode(){ Extension = null, DisasmFormat = "im 1",              Size = 2 }, // ed 76
            null                                                                          , // ed 77
            new Opcode(){ Extension = null, DisasmFormat = "in a,(c)",          Size = 2 }, // ed 78
            new Opcode(){ Extension = null, DisasmFormat = "out (c),a",         Size = 2 }, // ed 79
            new Opcode(){ Extension = null, DisasmFormat = "adc hl,sp",         Size = 2 }, // ed 7a
            new Opcode(){ Extension = null, DisasmFormat = "ld sp,(${0:x4})",   Size = 4 }, // ed 7b
            new Opcode(){ Extension = null, DisasmFormat = "neg",               Size = 2 }, // ed 7c
            new Opcode(){ Extension = null, DisasmFormat = "retn",              Size = 2 }, // ed 7d
            new Opcode(){ Extension = null, DisasmFormat = "im 2",              Size = 2 }, // ed 7e
            null                                                                          , // ed 7f
            null                                                                          , // ed 80
            null                                                                          , // ed 81
            null                                                                          , // ed 82
            null                                                                          , // ed 83
            null                                                                          , // ed 84
            null                                                                          , // ed 85
            null                                                                          , // ed 86
            null                                                                          , // ed 87
            null                                                                          , // ed 88
            null                                                                          , // ed 89
            null                                                                          , // ed 8a
            null                                                                          , // ed 8b
            null                                                                          , // ed 8c
            null                                                                          , // ed 8d
            null                                                                          , // ed 8e
            null                                                                          , // ed 8f
            null                                                                          , // ed 90
            null                                                                          , // ed 91
            null                                                                          , // ed 92
            null                                                                          , // ed 93
            null                                                                          , // ed 94
            null                                                                          , // ed 95
            null                                                                          , // ed 96
            null                                                                          , // ed 97
            null                                                                          , // ed 98
            null                                                                          , // ed 99
            null                                                                          , // ed 9a
            null                                                                          , // ed 9b
            null                                                                          , // ed 9c
            null                                                                          , // ed 9d
            null                                                                          , // ed 9e
            null                                                                          , // ed 9f
            new Opcode(){ Extension = null, DisasmFormat = "ldi",               Size = 2 }, // ed a0
            new Opcode(){ Extension = null, DisasmFormat = "cpi",               Size = 2 }, // ed a1
            new Opcode(){ Extension = null, DisasmFormat = "ini",               Size = 2 }, // ed a2
            new Opcode(){ Extension = null, DisasmFormat = "outi",              Size = 2 }, // ed a3
            null                                                                          , // ed a4
            null                                                                          , // ed a5
            null                                                                          , // ed a6
            null                                                                          , // ed a7
            new Opcode(){ Extension = null, DisasmFormat = "ldd",               Size = 2 }, // ed a8
            new Opcode(){ Extension = null, DisasmFormat = "cpd",               Size = 2 }, // ed a9
            new Opcode(){ Extension = null, DisasmFormat = "ind",               Size = 2 }, // ed aa
            new Opcode(){ Extension = null, DisasmFormat = "outd",              Size = 2 }, // ed ab
            null                                                                          , // ed ac
            null                                                                          , // ed ad
            null                                                                          , // ed ae
            null                                                                          , // ed af
            new Opcode(){ Extension = null, DisasmFormat = "ldir",              Size = 2 }, // ed b0
            new Opcode(){ Extension = null, DisasmFormat = "cpir",              Size = 2 }, // ed b1
            new Opcode(){ Extension = null, DisasmFormat = "inir",              Size = 2 }, // ed b2
            new Opcode(){ Extension = null, DisasmFormat = "otir",              Size = 2 }, // ed b3
            null                                                                          , // ed b4
            null                                                                          , // ed b5
            null                                                                          , // ed b6
            null                                                                          , // ed b7
            new Opcode(){ Extension = null, DisasmFormat = "lddr",              Size = 2 }, // ed b8
            new Opcode(){ Extension = null, DisasmFormat = "cpdr",              Size = 2 }, // ed b9
            new Opcode(){ Extension = null, DisasmFormat = "indr",              Size = 2 }, // ed ba
            new Opcode(){ Extension = null, DisasmFormat = "otdr",              Size = 2 }, // ed bb
            null                                                                          , // ed bc
            null                                                                          , // ed bd
            null                                                                          , // ed be
            null                                                                          , // ed bf
            null                                                                          , // ed c0
            null                                                                          , // ed c1
            null                                                                          , // ed c2
            null                                                                          , // ed c3
            null                                                                          , // ed c4
            null                                                                          , // ed c5
            null                                                                          , // ed c6
            null                                                                          , // ed c7
            null                                                                          , // ed c8
            null                                                                          , // ed c9
            null                                                                          , // ed ca
            null                                                                          , // ed cb
            null                                                                          , // ed cc
            null                                                                          , // ed cd
            null                                                                          , // ed ce
            null                                                                          , // ed cf
            null                                                                          , // ed d0
            null                                                                          , // ed d1
            null                                                                          , // ed d2
            null                                                                          , // ed d3
            null                                                                          , // ed d4
            null                                                                          , // ed d5
            null                                                                          , // ed d6
            null                                                                          , // ed d7
            null                                                                          , // ed d8
            null                                                                          , // ed d9
            null                                                                          , // ed da
            null                                                                          , // ed db
            null                                                                          , // ed dc
            null                                                                          , // ed dd
            null                                                                          , // ed de
            null                                                                          , // ed df
            null                                                                          , // ed e0
            null                                                                          , // ed e1
            null                                                                          , // ed e2
            null                                                                          , // ed e3
            null                                                                          , // ed e4
            null                                                                          , // ed e5
            null                                                                          , // ed e6
            null                                                                          , // ed e7
            null                                                                          , // ed e8
            null                                                                          , // ed e9
            null                                                                          , // ed ea
            null                                                                          , // ed eb
            null                                                                          , // ed ec
            null                                                                          , // ed ed
            null                                                                          , // ed ee
            null                                                                          , // ed ef
            null                                                                          , // ed f0
            null                                                                          , // ed f1
            null                                                                          , // ed f2
            null                                                                          , // ed f3
            null                                                                          , // ed f4
            null                                                                          , // ed f5
            null                                                                          , // ed f6
            null                                                                          , // ed f7
            null                                                                          , // ed f8
            null                                                                          , // ed f9
            null                                                                          , // ed fa
            null                                                                          , // ed fb
            null                                                                          , // ed fc
            null                                                                          , // ed fd
            null                                                                          , // ed fe
            null                                                                            // ed ff
        };

        private Opcode[] _ixBitsOpcodes = 
        {
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2}),b",    Size = 4 }, // dd cb 00
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2}),c",    Size = 4 }, // dd cb 01
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2}),d",    Size = 4 }, // dd cb 02
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2}),e",    Size = 4 }, // dd cb 03
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2}),h",    Size = 4 }, // dd cb 04
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2}),l",    Size = 4 }, // dd cb 05
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2})",      Size = 4 }, // dd cb 06
            new Opcode(){ Extension = null, DisasmFormat = "rlc (ix+${0:x2}),a",    Size = 4 }, // dd cb 07
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2}),b",    Size = 4 }, // dd cb 08
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2}),c",    Size = 4 }, // dd cb 09
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2}),d",    Size = 4 }, // dd cb 0a
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2}),e",    Size = 4 }, // dd cb 0b
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2}),h",    Size = 4 }, // dd cb 0c
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2}),l",    Size = 4 }, // dd cb 0d
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2})",      Size = 4 }, // dd cb 0e
            new Opcode(){ Extension = null, DisasmFormat = "rrc (ix+${0:x2}),a",    Size = 4 }, // dd cb 0f
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2}),b",     Size = 4 }, // dd cb 10
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2}),c",     Size = 4 }, // dd cb 11
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2}),d",     Size = 4 }, // dd cb 12
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2}),e",     Size = 4 }, // dd cb 13
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2}),h",     Size = 4 }, // dd cb 14
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2}),l",     Size = 4 }, // dd cb 15
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2})",       Size = 4 }, // dd cb 16
            new Opcode(){ Extension = null, DisasmFormat = "rl (ix+${0:x2}),a",     Size = 4 }, // dd cb 17
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2}),b",     Size = 4 }, // dd cb 18
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2}),c",     Size = 4 }, // dd cb 19
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2}),d",     Size = 4 }, // dd cb 1a
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2}),e",     Size = 4 }, // dd cb 1b
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2}),h",     Size = 4 }, // dd cb 1c
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2}),l",     Size = 4 }, // dd cb 1d
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2})",       Size = 4 }, // dd cb 1e
            new Opcode(){ Extension = null, DisasmFormat = "rr (ix+${0:x2}),a",     Size = 4 }, // dd cb 1f
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2}),b",    Size = 4 }, // dd cb 20
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2}),c",    Size = 4 }, // dd cb 21
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2}),d",    Size = 4 }, // dd cb 22
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2}),e",    Size = 4 }, // dd cb 23
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2}),h",    Size = 4 }, // dd cb 24
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2}),l",    Size = 4 }, // dd cb 25
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2})",      Size = 4 }, // dd cb 26
            new Opcode(){ Extension = null, DisasmFormat = "sla (ix+${0:x2}),a",    Size = 4 }, // dd cb 27
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2}),b",    Size = 4 }, // dd cb 28
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2}),c",    Size = 4 }, // dd cb 29
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2}),d",    Size = 4 }, // dd cb 2a
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2}),e",    Size = 4 }, // dd cb 2b
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2}),h",    Size = 4 }, // dd cb 2c
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2}),l",    Size = 4 }, // dd cb 2d
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2})",      Size = 4 }, // dd cb 2e
            new Opcode(){ Extension = null, DisasmFormat = "sra (ix+${0:x2}),a",    Size = 4 }, // dd cb 2f
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2}),b",    Size = 4 }, // dd cb 30
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2}),c",    Size = 4 }, // dd cb 31
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2}),d",    Size = 4 }, // dd cb 32
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2}),e",    Size = 4 }, // dd cb 33
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2}),h",    Size = 4 }, // dd cb 34
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2}),l",    Size = 4 }, // dd cb 35
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2})",      Size = 4 }, // dd cb 36
            new Opcode(){ Extension = null, DisasmFormat = "sll (ix+${0:x2}),a",    Size = 4 }, // dd cb 37
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2}),b",    Size = 4 }, // dd cb 38
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2}),c",    Size = 4 }, // dd cb 39
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2}),d",    Size = 4 }, // dd cb 3a
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2}),e",    Size = 4 }, // dd cb 3b
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2}),h",    Size = 4 }, // dd cb 3c
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2}),l",    Size = 4 }, // dd cb 3d
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2})",      Size = 4 }, // dd cb 3e
            new Opcode(){ Extension = null, DisasmFormat = "srl (ix+${0:x2}),a",    Size = 4 }, // dd cb 3f
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2}),b",  Size = 4 }, // dd cb 40
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2}),c",  Size = 4 }, // dd cb 41
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2}),d",  Size = 4 }, // dd cb 42
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2}),e",  Size = 4 }, // dd cb 43
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2}),h",  Size = 4 }, // dd cb 44
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2}),l",  Size = 4 }, // dd cb 45
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2})",    Size = 4 }, // dd cb 46
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(ix+${0:x2}),a",  Size = 4 }, // dd cb 47
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2}),b",  Size = 4 }, // dd cb 48
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2}),c",  Size = 4 }, // dd cb 49
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2}),d",  Size = 4 }, // dd cb 4a
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2}),e",  Size = 4 }, // dd cb 4b
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2}),h",  Size = 4 }, // dd cb 4c
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2}),l",  Size = 4 }, // dd cb 4d
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2})",    Size = 4 }, // dd cb 4e
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(ix+${0:x2}),a",  Size = 4 }, // dd cb 4f
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2}),b",  Size = 4 }, // dd cb 50
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2}),c",  Size = 4 }, // dd cb 51
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2}),d",  Size = 4 }, // dd cb 52
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2}),e",  Size = 4 }, // dd cb 53
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2}),h",  Size = 4 }, // dd cb 54
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2}),l",  Size = 4 }, // dd cb 55
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2})",    Size = 4 }, // dd cb 56
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(ix+${0:x2}),a",  Size = 4 }, // dd cb 57
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2}),b",  Size = 4 }, // dd cb 58
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2}),c",  Size = 4 }, // dd cb 59
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2}),d",  Size = 4 }, // dd cb 5a
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2}),e",  Size = 4 }, // dd cb 5b
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2}),h",  Size = 4 }, // dd cb 5c
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2}),l",  Size = 4 }, // dd cb 5d
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2})",    Size = 4 }, // dd cb 5e
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(ix+${0:x2}),a",  Size = 4 }, // dd cb 5f
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2}),b",  Size = 4 }, // dd cb 60
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2}),c",  Size = 4 }, // dd cb 61
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2}),d",  Size = 4 }, // dd cb 62
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2}),e",  Size = 4 }, // dd cb 63
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2}),h",  Size = 4 }, // dd cb 64
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2}),l",  Size = 4 }, // dd cb 65
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2})",    Size = 4 }, // dd cb 66
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(ix+${0:x2}),a",  Size = 4 }, // dd cb 67
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2}),b",  Size = 4 }, // dd cb 68
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2}),c",  Size = 4 }, // dd cb 69
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2}),d",  Size = 4 }, // dd cb 6a
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2}),e",  Size = 4 }, // dd cb 6b
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2}),h",  Size = 4 }, // dd cb 6c
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2}),l",  Size = 4 }, // dd cb 6d
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2})",    Size = 4 }, // dd cb 6e
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(ix+${0:x2}),a",  Size = 4 }, // dd cb 6f
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2}),b",  Size = 4 }, // dd cb 70
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2}),c",  Size = 4 }, // dd cb 71
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2}),d",  Size = 4 }, // dd cb 72
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2}),e",  Size = 4 }, // dd cb 73
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2}),h",  Size = 4 }, // dd cb 74
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2}),l",  Size = 4 }, // dd cb 75
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2})",    Size = 4 }, // dd cb 76
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(ix+${0:x2}),a",  Size = 4 }, // dd cb 77
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2}),b",  Size = 4 }, // dd cb 78
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2}),c",  Size = 4 }, // dd cb 79
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2}),d",  Size = 4 }, // dd cb 7a
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2}),e",  Size = 4 }, // dd cb 7b
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2}),h",  Size = 4 }, // dd cb 7c
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2}),l",  Size = 4 }, // dd cb 7d
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2})",    Size = 4 }, // dd cb 7e
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(ix+${0:x2}),a",  Size = 4 }, // dd cb 7f
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2}),b",  Size = 4 }, // dd cb 80
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2}),c",  Size = 4 }, // dd cb 81
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2}),d",  Size = 4 }, // dd cb 82
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2}),e",  Size = 4 }, // dd cb 83
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2}),h",  Size = 4 }, // dd cb 84
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2}),l",  Size = 4 }, // dd cb 85
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2})",    Size = 4 }, // dd cb 86
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(ix+${0:x2}),a",  Size = 4 }, // dd cb 87
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2}),b",  Size = 4 }, // dd cb 88
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2}),c",  Size = 4 }, // dd cb 89
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2}),d",  Size = 4 }, // dd cb 8a
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2}),e",  Size = 4 }, // dd cb 8b
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2}),h",  Size = 4 }, // dd cb 8c
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2}),l",  Size = 4 }, // dd cb 8d
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2})",    Size = 4 }, // dd cb 8e
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(ix+${0:x2}),a",  Size = 4 }, // dd cb 8f
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2}),b",  Size = 4 }, // dd cb 90
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2}),c",  Size = 4 }, // dd cb 91
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2}),d",  Size = 4 }, // dd cb 92
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2}),e",  Size = 4 }, // dd cb 93
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2}),h",  Size = 4 }, // dd cb 94
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2}),l",  Size = 4 }, // dd cb 95
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2})",    Size = 4 }, // dd cb 96
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(ix+${0:x2}),a",  Size = 4 }, // dd cb 97
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2}),b",  Size = 4 }, // dd cb 98
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2}),c",  Size = 4 }, // dd cb 99
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2}),d",  Size = 4 }, // dd cb 9a
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2}),e",  Size = 4 }, // dd cb 9b
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2}),h",  Size = 4 }, // dd cb 9c
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2}),l",  Size = 4 }, // dd cb 9d
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2})",    Size = 4 }, // dd cb 9e
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(ix+${0:x2}),a",  Size = 4 }, // dd cb 9f
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2}),b",  Size = 4 }, // dd cb a0
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2}),c",  Size = 4 }, // dd cb a1
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2}),d",  Size = 4 }, // dd cb a2
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2}),e",  Size = 4 }, // dd cb a3
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2}),h",  Size = 4 }, // dd cb a4
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2}),l",  Size = 4 }, // dd cb a5
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2})",    Size = 4 }, // dd cb a6
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(ix+${0:x2}),a",  Size = 4 }, // dd cb a7
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2}),b",  Size = 4 }, // dd cb a8
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2}),c",  Size = 4 }, // dd cb a9
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2}),d",  Size = 4 }, // dd cb aa
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2}),e",  Size = 4 }, // dd cb ab
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2}),h",  Size = 4 }, // dd cb ac
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2}),l",  Size = 4 }, // dd cb ad
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2})",    Size = 4 }, // dd cb ae
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(ix+${0:x2}),a",  Size = 4 }, // dd cb af
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2}),b",  Size = 4 }, // dd cb b0
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2}),c",  Size = 4 }, // dd cb b1
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2}),d",  Size = 4 }, // dd cb b2
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2}),e",  Size = 4 }, // dd cb b3
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2}),h",  Size = 4 }, // dd cb b4
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2}),l",  Size = 4 }, // dd cb b5
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2})",    Size = 4 }, // dd cb b6
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(ix+${0:x2}),a",  Size = 4 }, // dd cb b7
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2}),b",  Size = 4 }, // dd cb b8
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2}),c",  Size = 4 }, // dd cb b9
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2}),d",  Size = 4 }, // dd cb ba
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2}),e",  Size = 4 }, // dd cb bb
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2}),h",  Size = 4 }, // dd cb bc
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2}),l",  Size = 4 }, // dd cb bd
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2})",    Size = 4 }, // dd cb be
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(ix+${0:x2}),a",  Size = 4 }, // dd cb bf
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2}),b",  Size = 4 }, // dd cb c0
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2}),c",  Size = 4 }, // dd cb c1
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2}),d",  Size = 4 }, // dd cb c2
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2}),e",  Size = 4 }, // dd cb c3
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2}),h",  Size = 4 }, // dd cb c4
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2}),l",  Size = 4 }, // dd cb c5
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2})",    Size = 4 }, // dd cb c6
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(ix+${0:x2}),a",  Size = 4 }, // dd cb c7
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2}),b",  Size = 4 }, // dd cb c8
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2}),c",  Size = 4 }, // dd cb c9
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2}),d",  Size = 4 }, // dd cb ca
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2}),e",  Size = 4 }, // dd cb cb
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2}),h",  Size = 4 }, // dd cb cc
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2}),l",  Size = 4 }, // dd cb cd
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2})",    Size = 4 }, // dd cb ce
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(ix+${0:x2}),a",  Size = 4 }, // dd cb cf
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2}),b",  Size = 4 }, // dd cb d0
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2}),c",  Size = 4 }, // dd cb d1
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2}),d",  Size = 4 }, // dd cb d2
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2}),e",  Size = 4 }, // dd cb d3
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2}),h",  Size = 4 }, // dd cb d4
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2}),l",  Size = 4 }, // dd cb d5
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2})",    Size = 4 }, // dd cb d6
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(ix+${0:x2}),a",  Size = 4 }, // dd cb d7
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2}),b",  Size = 4 }, // dd cb d8
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2}),c",  Size = 4 }, // dd cb d9
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2}),d",  Size = 4 }, // dd cb da
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2}),e",  Size = 4 }, // dd cb db
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2}),h",  Size = 4 }, // dd cb dc
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2}),l",  Size = 4 }, // dd cb dd
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2})",    Size = 4 }, // dd cb de
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(ix+${0:x2}),a",  Size = 4 }, // dd cb df
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2}),b",  Size = 4 }, // dd cb e0
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2}),c",  Size = 4 }, // dd cb e1
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2}),d",  Size = 4 }, // dd cb e2
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2}),e",  Size = 4 }, // dd cb e3
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2}),h",  Size = 4 }, // dd cb e4
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2}),l",  Size = 4 }, // dd cb e5
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2})",    Size = 4 }, // dd cb e6
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(ix+${0:x2}),a",  Size = 4 }, // dd cb e7
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2}),b",  Size = 4 }, // dd cb e8
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2}),c",  Size = 4 }, // dd cb e9
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2}),d",  Size = 4 }, // dd cb ea
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2}),e",  Size = 4 }, // dd cb eb
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2}),h",  Size = 4 }, // dd cb ec
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2}),l",  Size = 4 }, // dd cb ed
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2})",    Size = 4 }, // dd cb ee
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(ix+${0:x2}),a",  Size = 4 }, // dd cb ef
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2}),b",  Size = 4 }, // dd cb f0
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2}),c",  Size = 4 }, // dd cb f1
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2}),d",  Size = 4 }, // dd cb f2
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2}),e",  Size = 4 }, // dd cb f3
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2}),h",  Size = 4 }, // dd cb f4
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2}),l",  Size = 4 }, // dd cb f5
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2})",    Size = 4 }, // dd cb f6
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(ix+${0:x2}),a",  Size = 4 }, // dd cb f7
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2}),b",  Size = 4 }, // dd cb f8
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2}),c",  Size = 4 }, // dd cb f9
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2}),d",  Size = 4 }, // dd cb fa
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2}),e",  Size = 4 }, // dd cb fb
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2}),h",  Size = 4 }, // dd cb fc
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2}),l",  Size = 4 }, // dd cb fd
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2})",    Size = 4 }, // dd cb fe
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(ix+${0:x2}),a",  Size = 4 }  // dd cb ff
        };

        private Opcode[] _iyBitsOpcodes =
        {
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2}),b",    Size = 4 }, // fd cb 00
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2}),c",    Size = 4 }, // fd cb 01
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2}),d",    Size = 4 }, // fd cb 02
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2}),e",    Size = 4 }, // fd cb 03
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2}),h",    Size = 4 }, // fd cb 04
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2}),l",    Size = 4 }, // fd cb 05
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2})",      Size = 4 }, // fd cb 06
            new Opcode(){ Extension = null, DisasmFormat = "rlc (iy+${0:x2}),a",    Size = 4 }, // fd cb 07
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2}),b",    Size = 4 }, // fd cb 08
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2}),c",    Size = 4 }, // fd cb 09
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2}),d",    Size = 4 }, // fd cb 0a
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2}),e",    Size = 4 }, // fd cb 0b
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2}),h",    Size = 4 }, // fd cb 0c
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2}),l",    Size = 4 }, // fd cb 0d
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2})",      Size = 4 }, // fd cb 0e
            new Opcode(){ Extension = null, DisasmFormat = "rrc (iy+${0:x2}),a",    Size = 4 }, // fd cb 0f
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2}),b",     Size = 4 }, // fd cb 10
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2}),c",     Size = 4 }, // fd cb 11
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2}),d",     Size = 4 }, // fd cb 12
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2}),e",     Size = 4 }, // fd cb 13
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2}),h",     Size = 4 }, // fd cb 14
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2}),l",     Size = 4 }, // fd cb 15
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2})",       Size = 4 }, // fd cb 16
            new Opcode(){ Extension = null, DisasmFormat = "rl (iy+${0:x2}),a",     Size = 4 }, // fd cb 17
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2}),b",     Size = 4 }, // fd cb 18
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2}),c",     Size = 4 }, // fd cb 19
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2}),d",     Size = 4 }, // fd cb 1a
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2}),e",     Size = 4 }, // fd cb 1b
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2}),h",     Size = 4 }, // fd cb 1c
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2}),l",     Size = 4 }, // fd cb 1d
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2})",       Size = 4 }, // fd cb 1e
            new Opcode(){ Extension = null, DisasmFormat = "rr (iy+${0:x2}),a",     Size = 4 }, // fd cb 1f
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2}),b",    Size = 4 }, // fd cb 20
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2}),c",    Size = 4 }, // fd cb 21
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2}),d",    Size = 4 }, // fd cb 22
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2}),e",    Size = 4 }, // fd cb 23
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2}),h",    Size = 4 }, // fd cb 24
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2}),l",    Size = 4 }, // fd cb 25
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2})",      Size = 4 }, // fd cb 26
            new Opcode(){ Extension = null, DisasmFormat = "sla (iy+${0:x2}),a",    Size = 4 }, // fd cb 27
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2}),b",    Size = 4 }, // fd cb 28
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2}),c",    Size = 4 }, // fd cb 29
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2}),d",    Size = 4 }, // fd cb 2a
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2}),e",    Size = 4 }, // fd cb 2b
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2}),h",    Size = 4 }, // fd cb 2c
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2}),l",    Size = 4 }, // fd cb 2d
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2})",      Size = 4 }, // fd cb 2e
            new Opcode(){ Extension = null, DisasmFormat = "sra (iy+${0:x2}),a",    Size = 4 }, // fd cb 2f
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2}),b",    Size = 4 }, // fd cb 30
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2}),c",    Size = 4 }, // fd cb 31
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2}),d",    Size = 4 }, // fd cb 32
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2}),e",    Size = 4 }, // fd cb 33
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2}),h",    Size = 4 }, // fd cb 34
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2}),l",    Size = 4 }, // fd cb 35
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2})",      Size = 4 }, // fd cb 36
            new Opcode(){ Extension = null, DisasmFormat = "sll (iy+${0:x2}),a",    Size = 4 }, // fd cb 37
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2}),b",    Size = 4 }, // fd cb 38
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2}),c",    Size = 4 }, // fd cb 39
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2}),d",    Size = 4 }, // fd cb 3a
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2}),e",    Size = 4 }, // fd cb 3b
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2}),h",    Size = 4 }, // fd cb 3c
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2}),l",    Size = 4 }, // fd cb 3d
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2})",      Size = 4 }, // fd cb 3e
            new Opcode(){ Extension = null, DisasmFormat = "srl (iy+${0:x2}),a",    Size = 4 }, // fd cb 3f
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2}),b",  Size = 4 }, // fd cb 40
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2}),c",  Size = 4 }, // fd cb 41
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2}),d",  Size = 4 }, // fd cb 42
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2}),e",  Size = 4 }, // fd cb 43
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2}),h",  Size = 4 }, // fd cb 44
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2}),l",  Size = 4 }, // fd cb 45
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2})",    Size = 4 }, // fd cb 46
            new Opcode(){ Extension = null, DisasmFormat = "bit 0,(iy+${0:x2}),a",  Size = 4 }, // fd cb 47
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2}),b",  Size = 4 }, // fd cb 48
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2}),c",  Size = 4 }, // fd cb 49
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2}),d",  Size = 4 }, // fd cb 4a
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2}),e",  Size = 4 }, // fd cb 4b
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2}),h",  Size = 4 }, // fd cb 4c
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2}),l",  Size = 4 }, // fd cb 4d
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2})",    Size = 4 }, // fd cb 4e
            new Opcode(){ Extension = null, DisasmFormat = "bit 1,(iy+${0:x2}),a",  Size = 4 }, // fd cb 4f
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2}),b",  Size = 4 }, // fd cb 50
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2}),c",  Size = 4 }, // fd cb 51
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2}),d",  Size = 4 }, // fd cb 52
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2}),e",  Size = 4 }, // fd cb 53
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2}),h",  Size = 4 }, // fd cb 54
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2}),l",  Size = 4 }, // fd cb 55
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2})",    Size = 4 }, // fd cb 56
            new Opcode(){ Extension = null, DisasmFormat = "bit 2,(iy+${0:x2}),a",  Size = 4 }, // fd cb 57
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2}),b",  Size = 4 }, // fd cb 58
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2}),c",  Size = 4 }, // fd cb 59
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2}),d",  Size = 4 }, // fd cb 5a
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2}),e",  Size = 4 }, // fd cb 5b
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2}),h",  Size = 4 }, // fd cb 5c
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2}),l",  Size = 4 }, // fd cb 5d
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2})",    Size = 4 }, // fd cb 5e
            new Opcode(){ Extension = null, DisasmFormat = "bit 3,(iy+${0:x2}),a",  Size = 4 }, // fd cb 5f
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2}),b",  Size = 4 }, // fd cb 60
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2}),c",  Size = 4 }, // fd cb 61
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2}),d",  Size = 4 }, // fd cb 62
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2}),e",  Size = 4 }, // fd cb 63
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2}),h",  Size = 4 }, // fd cb 64
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2}),l",  Size = 4 }, // fd cb 65
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2})",    Size = 4 }, // fd cb 66
            new Opcode(){ Extension = null, DisasmFormat = "bit 4,(iy+${0:x2}),a",  Size = 4 }, // fd cb 67
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2}),b",  Size = 4 }, // fd cb 68
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2}),c",  Size = 4 }, // fd cb 69
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2}),d",  Size = 4 }, // fd cb 6a
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2}),e",  Size = 4 }, // fd cb 6b
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2}),h",  Size = 4 }, // fd cb 6c
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2}),l",  Size = 4 }, // fd cb 6d
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2})",    Size = 4 }, // fd cb 6e
            new Opcode(){ Extension = null, DisasmFormat = "bit 5,(iy+${0:x2}),a",  Size = 4 }, // fd cb 6f
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2}),b",  Size = 4 }, // fd cb 70
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2}),c",  Size = 4 }, // fd cb 71
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2}),d",  Size = 4 }, // fd cb 72
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2}),e",  Size = 4 }, // fd cb 73
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2}),h",  Size = 4 }, // fd cb 74
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2}),l",  Size = 4 }, // fd cb 75
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2})",    Size = 4 }, // fd cb 76
            new Opcode(){ Extension = null, DisasmFormat = "bit 6,(iy+${0:x2}),a",  Size = 4 }, // fd cb 77
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2}),b",  Size = 4 }, // fd cb 78
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2}),c",  Size = 4 }, // fd cb 79
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2}),d",  Size = 4 }, // fd cb 7a
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2}),e",  Size = 4 }, // fd cb 7b
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2}),h",  Size = 4 }, // fd cb 7c
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2}),l",  Size = 4 }, // fd cb 7d
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2})",    Size = 4 }, // fd cb 7e
            new Opcode(){ Extension = null, DisasmFormat = "bit 7,(iy+${0:x2}),a",  Size = 4 }, // fd cb 7f
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2}),b",  Size = 4 }, // fd cb 80
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2}),c",  Size = 4 }, // fd cb 81
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2}),d",  Size = 4 }, // fd cb 82
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2}),e",  Size = 4 }, // fd cb 83
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2}),h",  Size = 4 }, // fd cb 84
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2}),l",  Size = 4 }, // fd cb 85
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2})",    Size = 4 }, // fd cb 86
            new Opcode(){ Extension = null, DisasmFormat = "res 0,(iy+${0:x2}),a",  Size = 4 }, // fd cb 87
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2}),b",  Size = 4 }, // fd cb 88
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2}),c",  Size = 4 }, // fd cb 89
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2}),d",  Size = 4 }, // fd cb 8a
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2}),e",  Size = 4 }, // fd cb 8b
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2}),h",  Size = 4 }, // fd cb 8c
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2}),l",  Size = 4 }, // fd cb 8d
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2})",    Size = 4 }, // fd cb 8e
            new Opcode(){ Extension = null, DisasmFormat = "res 1,(iy+${0:x2}),a",  Size = 4 }, // fd cb 8f
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2}),b",  Size = 4 }, // fd cb 90
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2}),c",  Size = 4 }, // fd cb 91
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2}),d",  Size = 4 }, // fd cb 92
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2}),e",  Size = 4 }, // fd cb 93
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2}),h",  Size = 4 }, // fd cb 94
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2}),l",  Size = 4 }, // fd cb 95
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2})",    Size = 4 }, // fd cb 96
            new Opcode(){ Extension = null, DisasmFormat = "res 2,(iy+${0:x2}),a",  Size = 4 }, // fd cb 97
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2}),b",  Size = 4 }, // fd cb 98
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2}),c",  Size = 4 }, // fd cb 99
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2}),d",  Size = 4 }, // fd cb 9a
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2}),e",  Size = 4 }, // fd cb 9b
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2}),h",  Size = 4 }, // fd cb 9c
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2}),l",  Size = 4 }, // fd cb 9d
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2})",    Size = 4 }, // fd cb 9e
            new Opcode(){ Extension = null, DisasmFormat = "res 3,(iy+${0:x2}),a",  Size = 4 }, // fd cb 9f
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2}),b",  Size = 4 }, // fd cb a0
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2}),c",  Size = 4 }, // fd cb a1
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2}),d",  Size = 4 }, // fd cb a2
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2}),e",  Size = 4 }, // fd cb a3
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2}),h",  Size = 4 }, // fd cb a4
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2}),l",  Size = 4 }, // fd cb a5
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2})",    Size = 4 }, // fd cb a6
            new Opcode(){ Extension = null, DisasmFormat = "res 4,(iy+${0:x2}),a",  Size = 4 }, // fd cb a7
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2}),b",  Size = 4 }, // fd cb a8
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2}),c",  Size = 4 }, // fd cb a9
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2}),d",  Size = 4 }, // fd cb aa
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2}),e",  Size = 4 }, // fd cb ab
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2}),h",  Size = 4 }, // fd cb ac
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2}),l",  Size = 4 }, // fd cb ad
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2})",    Size = 4 }, // fd cb ae
            new Opcode(){ Extension = null, DisasmFormat = "res 5,(iy+${0:x2}),a",  Size = 4 }, // fd cb af
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2}),b",  Size = 4 }, // fd cb b0
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2}),c",  Size = 4 }, // fd cb b1
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2}),d",  Size = 4 }, // fd cb b2
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2}),e",  Size = 4 }, // fd cb b3
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2}),h",  Size = 4 }, // fd cb b4
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2}),l",  Size = 4 }, // fd cb b5
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2})",    Size = 4 }, // fd cb b6
            new Opcode(){ Extension = null, DisasmFormat = "res 6,(iy+${0:x2}),a",  Size = 4 }, // fd cb b7
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2}),b",  Size = 4 }, // fd cb b8
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2}),c",  Size = 4 }, // fd cb b9
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2}),d",  Size = 4 }, // fd cb ba
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2}),e",  Size = 4 }, // fd cb bb
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2}),h",  Size = 4 }, // fd cb bc
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2}),l",  Size = 4 }, // fd cb bd
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2})",    Size = 4 }, // fd cb be
            new Opcode(){ Extension = null, DisasmFormat = "res 7,(iy+${0:x2}),a",  Size = 4 }, // fd cb bf
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2}),b",  Size = 4 }, // fd cb c0
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2}),c",  Size = 4 }, // fd cb c1
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2}),d",  Size = 4 }, // fd cb c2
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2}),e",  Size = 4 }, // fd cb c3
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2}),h",  Size = 4 }, // fd cb c4
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2}),l",  Size = 4 }, // fd cb c5
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2})",    Size = 4 }, // fd cb c6
            new Opcode(){ Extension = null, DisasmFormat = "set 0,(iy+${0:x2}),a",  Size = 4 }, // fd cb c7
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2}),b",  Size = 4 }, // fd cb c8
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2}),c",  Size = 4 }, // fd cb c9
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2}),d",  Size = 4 }, // fd cb ca
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2}),e",  Size = 4 }, // fd cb cb
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2}),h",  Size = 4 }, // fd cb cc
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2}),l",  Size = 4 }, // fd cb cd
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2})",    Size = 4 }, // fd cb ce
            new Opcode(){ Extension = null, DisasmFormat = "set 1,(iy+${0:x2}),a",  Size = 4 }, // fd cb cf
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2}),b",  Size = 4 }, // fd cb d0
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2}),c",  Size = 4 }, // fd cb d1
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2}),d",  Size = 4 }, // fd cb d2
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2}),e",  Size = 4 }, // fd cb d3
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2}),h",  Size = 4 }, // fd cb d4
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2}),l",  Size = 4 }, // fd cb d5
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2})",    Size = 4 }, // fd cb d6
            new Opcode(){ Extension = null, DisasmFormat = "set 2,(iy+${0:x2}),a",  Size = 4 }, // fd cb d7
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2}),b",  Size = 4 }, // fd cb d8
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2}),c",  Size = 4 }, // fd cb d9
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2}),d",  Size = 4 }, // fd cb da
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2}),e",  Size = 4 }, // fd cb db
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2}),h",  Size = 4 }, // fd cb dc
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2}),l",  Size = 4 }, // fd cb dd
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2})",    Size = 4 }, // fd cb de
            new Opcode(){ Extension = null, DisasmFormat = "set 3,(iy+${0:x2}),a",  Size = 4 }, // fd cb df
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2}),b",  Size = 4 }, // fd cb e0
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2}),c",  Size = 4 }, // fd cb e1
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2}),d",  Size = 4 }, // fd cb e2
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2}),e",  Size = 4 }, // fd cb e3
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2}),h",  Size = 4 }, // fd cb e4
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2}),l",  Size = 4 }, // fd cb e5
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2})",    Size = 4 }, // fd cb e6
            new Opcode(){ Extension = null, DisasmFormat = "set 4,(iy+${0:x2}),a",  Size = 4 }, // fd cb e7
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2}),b",  Size = 4 }, // fd cb e8
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2}),c",  Size = 4 }, // fd cb e9
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2}),d",  Size = 4 }, // fd cb ea
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2}),e",  Size = 4 }, // fd cb eb
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2}),h",  Size = 4 }, // fd cb ec
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2}),l",  Size = 4 }, // fd cb ed
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2})",    Size = 4 }, // fd cb ee
            new Opcode(){ Extension = null, DisasmFormat = "set 5,(iy+${0:x2}),a",  Size = 4 }, // fd cb ef
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2}),b",  Size = 4 }, // fd cb f0
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2}),c",  Size = 4 }, // fd cb f1
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2}),d",  Size = 4 }, // fd cb f2
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2}),e",  Size = 4 }, // fd cb f3
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2}),h",  Size = 4 }, // fd cb f4
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2}),l",  Size = 4 }, // fd cb f5
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2})",    Size = 4 }, // fd cb f6
            new Opcode(){ Extension = null, DisasmFormat = "set 6,(iy+${0:x2}),a",  Size = 4 }, // fd cb f7
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2}),b",  Size = 4 }, // fd cb f8
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2}),c",  Size = 4 }, // fd cb f9
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2}),d",  Size = 4 }, // fd cb fa
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2}),e",  Size = 4 }, // fd cb fb
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2}),h",  Size = 4 }, // fd cb fc
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2}),l",  Size = 4 }, // fd cb fd
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2})",    Size = 4 }, // fd cb fe
            new Opcode(){ Extension = null, DisasmFormat = "set 7,(iy+${0:x2}),a",  Size = 4 }  // fd cb ff
        };

        private Opcode[] _ixOpcodes = 
        {
            null,                                                                                   // dd 00
            null,                                                                                   // dd 01
            null,                                                                                   // dd 02
            null,                                                                                   // dd 03
            null,                                                                                   // dd 04
            null,                                                                                   // dd 05
            null,                                                                                   // dd 06
            null,                                                                                   // dd 07
            null,                                                                                   // dd 08
            new Opcode(){ Extension = null, DisasmFormat = "add ix,bc",                 Size = 2 }, // dd 09
            null,                                                                                   // dd 0a
            null,                                                                                   // dd 0b
            null,                                                                                   // dd 0c
            null,                                                                                   // dd 0d
            null,                                                                                   // dd 0e
            null,                                                                                   // dd 0f
            null,                                                                                   // dd 10
            null,                                                                                   // dd 11
            null,                                                                                   // dd 12
            null,                                                                                   // dd 13
            null,                                                                                   // dd 14
            null,                                                                                   // dd 15
            null,                                                                                   // dd 16
            null,                                                                                   // dd 17
            null,                                                                                   // dd 18
            new Opcode(){ Extension = null, DisasmFormat = "add ix,de",                 Size = 2 }, // dd 19
            null,                                                                                   // dd 1a
            null,                                                                                   // dd 1b
            null,                                                                                   // dd 1c
            null,                                                                                   // dd 1d
            null,                                                                                   // dd 1e
            null,                                                                                   // dd 1f
            null,                                                                                   // dd 20
            new Opcode(){ Extension = null, DisasmFormat = "ld ix,${0:x4}",             Size = 4 }, // dd 21
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),ix",           Size = 4 }, // dd 22
            new Opcode(){ Extension = null, DisasmFormat = "inc ix",                    Size = 2 }, // dd 23
            new Opcode(){ Extension = null, DisasmFormat = "inc ixh",                   Size = 2 }, // dd 24
            new Opcode(){ Extension = null, DisasmFormat = "dec ixh",                   Size = 2 }, // dd 25
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,${0:x2}",            Size = 3 }, // dd 26
            null,                                                                                   // dd 27
            null,                                                                                   // dd 28
            new Opcode(){ Extension = null, DisasmFormat = "add ix,ix",                 Size = 2 }, // dd 29
            new Opcode(){ Extension = null, DisasmFormat = "ld ix,(${0:x4})",           Size = 4 }, // dd 2a
            new Opcode(){ Extension = null, DisasmFormat = "dec ix",                    Size = 2 }, // dd 2b
            new Opcode(){ Extension = null, DisasmFormat = "inc ixl",                   Size = 2 }, // dd 2c
            new Opcode(){ Extension = null, DisasmFormat = "dec ixl",                   Size = 2 }, // dd 2d
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,${0:x2}",            Size = 3 }, // dd 2e
            null,                                                                                   // dd 2f
            null,                                                                                   // dd 30
            null,                                                                                   // dd 31
            null,                                                                                   // dd 32
            null,                                                                                   // dd 33
            new Opcode(){ Extension = null, DisasmFormat = "inc (ix+${0:x2})",          Size = 3 }, // dd 34
            new Opcode(){ Extension = null, DisasmFormat = "dec (ix+${0:x2})",          Size = 3 }, // dd 35
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),${1:x2}",   Size = 4 }, // dd 36
            null,                                                                                   // dd 37
            null,                                                                                   // dd 38
            new Opcode(){ Extension = null, DisasmFormat = "add ix,sp",                 Size = 2 }, // dd 39
            null,                                                                                   // dd 3a
            null,                                                                                   // dd 3b
            null,                                                                                   // dd 3c
            null,                                                                                   // dd 3d
            null,                                                                                   // dd 3e
            null,                                                                                   // dd 3f
            null,                                                                                   // dd 40
            null,                                                                                   // dd 41
            null,                                                                                   // dd 42
            null,                                                                                   // dd 43
            new Opcode(){ Extension = null, DisasmFormat = "ld b,ixh",                  Size = 2 }, // dd 44
            new Opcode(){ Extension = null, DisasmFormat = "ld b,ixl",                  Size = 2 }, // dd 45
            new Opcode(){ Extension = null, DisasmFormat = "ld b,(ix+${0:x2})",         Size = 3 }, // dd 46
            null,                                                                                   // dd 47
            null,                                                                                   // dd 48
            null,                                                                                   // dd 49
            null,                                                                                   // dd 4a
            null,                                                                                   // dd 4b
            new Opcode(){ Extension = null, DisasmFormat = "ld c,ixh",                  Size = 2 }, // dd 4c
            new Opcode(){ Extension = null, DisasmFormat = "ld c,ixl",                  Size = 2 }, // dd 4d
            new Opcode(){ Extension = null, DisasmFormat = "ld c,(ix+${0:x2})",         Size = 3 }, // dd 4e
            null,                                                                                   // dd 4f
            null,                                                                                   // dd 50
            null,                                                                                   // dd 51
            null,                                                                                   // dd 52
            null,                                                                                   // dd 53
            new Opcode(){ Extension = null, DisasmFormat = "ld d,ixh",                  Size = 2 }, // dd 54
            new Opcode(){ Extension = null, DisasmFormat = "ld d,ixl",                  Size = 2 }, // dd 55
            new Opcode(){ Extension = null, DisasmFormat = "ld d,(ix+${0:x2})",         Size = 3 }, // dd 56
            null,                                                                                   // dd 57
            null,                                                                                   // dd 58
            null,                                                                                   // dd 59
            null,                                                                                   // dd 5a
            null,                                                                                   // dd 5b
            new Opcode(){ Extension = null, DisasmFormat = "ld e,ixh",                  Size = 2 }, // dd 5c
            new Opcode(){ Extension = null, DisasmFormat = "ld e,ixl",                  Size = 2 }, // dd 5d
            new Opcode(){ Extension = null, DisasmFormat = "ld e,(ix+${0:x2})",         Size = 3 }, // dd 5e
            null,                                                                                   // dd 5f
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,b",                  Size = 2 }, // dd 60
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,c",                  Size = 2 }, // dd 61
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,d",                  Size = 2 }, // dd 62
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,e",                  Size = 2 }, // dd 63
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,ixh",                  Size = 2 }, // dd 64
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,ixl",                  Size = 2 }, // dd 65
            new Opcode(){ Extension = null, DisasmFormat = "ld h,(ix+${0:x2})",         Size = 3 }, // dd 66
            new Opcode(){ Extension = null, DisasmFormat = "ld ixh,a",                  Size = 2 }, // dd 67
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,b",                  Size = 2 }, // dd 68
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,c",                  Size = 2 }, // dd 69
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,d",                  Size = 2 }, // dd 6a
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,e",                  Size = 2 }, // dd 6b
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,ixh",                  Size = 2 }, // dd 6c
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,ixl",                  Size = 2 }, // dd 6d
            new Opcode(){ Extension = null, DisasmFormat = "ld l,(ix+${0:x2})",         Size = 3 }, // dd 6e
            new Opcode(){ Extension = null, DisasmFormat = "ld ixl,a",                  Size = 2 }, // dd 6f
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),b",         Size = 3 }, // dd 70
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),c",         Size = 3 }, // dd 71
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),d",         Size = 3 }, // dd 72
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),e",         Size = 3 }, // dd 73
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),h",         Size = 3 }, // dd 74
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),l",         Size = 3 }, // dd 75
            null,                                                                                   // dd 76
            new Opcode(){ Extension = null, DisasmFormat = "ld (ix+${0:x2}),a",         Size = 3 }, // dd 77
            null,                                                                                   // dd 78
            null,                                                                                   // dd 79
            null,                                                                                   // dd 7a
            null,                                                                                   // dd 7b
            new Opcode(){ Extension = null, DisasmFormat = "ld a,ixh",                  Size = 2 }, // dd 7c
            new Opcode(){ Extension = null, DisasmFormat = "ld a,ixl",                  Size = 2 }, // dd 7d
            new Opcode(){ Extension = null, DisasmFormat = "ld a,(ix+${0:x2})",         Size = 3 }, // dd 7e
            null,                                                                                   // dd 7f
            null,                                                                                   // dd 80
            null,                                                                                   // dd 81
            null,                                                                                   // dd 82
            null,                                                                                   // dd 83
            new Opcode(){ Extension = null, DisasmFormat = "add a,ixh",                 Size = 2 }, // dd 84
            new Opcode(){ Extension = null, DisasmFormat = "add a,ixl",                 Size = 2 }, // dd 85
            new Opcode(){ Extension = null, DisasmFormat = "add a,(ix+${0:x2})",        Size = 3 }, // dd 86
            null,                                                                                   // dd 87
            null,                                                                                   // dd 88
            null,                                                                                   // dd 89
            null,                                                                                   // dd 8a
            null,                                                                                   // dd 8b
            new Opcode(){ Extension = null, DisasmFormat = "adc a,ixh",                 Size = 2 }, // dd 8c
            new Opcode(){ Extension = null, DisasmFormat = "adc a,ixl",                 Size = 2 }, // dd 8d
            new Opcode(){ Extension = null, DisasmFormat = "adc a,(ix+${0:x2})",        Size = 3 }, // dd 8e
            null,                                                                                   // dd 8f
            null,                                                                                   // dd 90
            null,                                                                                   // dd 91
            null,                                                                                   // dd 92
            null,                                                                                   // dd 93
            new Opcode(){ Extension = null, DisasmFormat = "sub ixh",                   Size = 2 }, // dd 94
            new Opcode(){ Extension = null, DisasmFormat = "sub ixl",                   Size = 2 }, // dd 95
            new Opcode(){ Extension = null, DisasmFormat = "sub (ix+${0:x2})",          Size = 3 }, // dd 96
            null,                                                                                   // dd 97
            null,                                                                                   // dd 98
            null,                                                                                   // dd 99
            null,                                                                                   // dd 9a
            null,                                                                                   // dd 9b
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,ixh",                 Size = 2 }, // dd 9c
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,ixl",                 Size = 2 }, // dd 9d
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,(ix+${0:x2})",        Size = 3 }, // dd 9e
            null,                                                                                   // dd 9f
            null,                                                                                   // dd a0
            null,                                                                                   // dd a1
            null,                                                                                   // dd a2
            null,                                                                                   // dd a3
            new Opcode(){ Extension = null, DisasmFormat = "and ixh",                   Size = 2 }, // dd a4
            new Opcode(){ Extension = null, DisasmFormat = "and ixl",                   Size = 2 }, // dd a5
            new Opcode(){ Extension = null, DisasmFormat = "and (ix+${0:x2})",          Size = 3 }, // dd a6
            null,                                                                                   // dd a7
            null,                                                                                   // dd a8
            null,                                                                                   // dd a9
            null,                                                                                   // dd aa
            null,                                                                                   // dd ab
            new Opcode(){ Extension = null, DisasmFormat = "xor ixh",                   Size = 2 }, // dd ac
            new Opcode(){ Extension = null, DisasmFormat = "xor ixl",                   Size = 2 }, // dd ad
            new Opcode(){ Extension = null, DisasmFormat = "xor (ix+${0:x2})",          Size = 3 }, // dd ae            
            null,                                                                                   // dd af
            null,                                                                                   // dd b0
            null,                                                                                   // dd b1
            null,                                                                                   // dd b2
            null,                                                                                   // dd b3
            new Opcode(){ Extension = null, DisasmFormat = "or ixh",                    Size = 2 }, // dd b4
            new Opcode(){ Extension = null, DisasmFormat = "or ixl",                    Size = 2 }, // dd b5
            new Opcode(){ Extension = null, DisasmFormat = "or (ix+${0:x2})",           Size = 3 }, // dd b6
            null,                                                                                   // dd b7
            null,                                                                                   // dd b8
            null,                                                                                   // dd b9
            null,                                                                                   // dd ba
            null,                                                                                   // dd bb
            new Opcode(){ Extension = null, DisasmFormat = "cp ixh",                    Size = 2 }, // dd bc
            new Opcode(){ Extension = null, DisasmFormat = "cp ixl",                    Size = 2 }, // dd bd
            new Opcode(){ Extension = null, DisasmFormat = "cp (ix+${0:x2})",           Size = 3 }, // dd be
            null,                                                                                   // dd bf
            null,                                                                                   // dd c0
            null,                                                                                   // dd c1
            null,                                                                                   // dd c2
            null,                                                                                   // dd c3
            null,                                                                                   // dd c4
            null,                                                                                   // dd c5
            null,                                                                                   // dd c6
            null,                                                                                   // dd c7
            null,                                                                                   // dd c8
            null,                                                                                   // dd c9
            null,                                                                                   // dd ca
            new Opcode(){ Extension = null, DisasmFormat = null,                        Size = 3 }, // dd cb
            null,                                                                                   // dd cc
            null,                                                                                   // dd cd
            null,                                                                                   // dd ce
            null,                                                                                   // dd cf
            null,                                                                                   // dd d0
            null,                                                                                   // dd d1
            null,                                                                                   // dd d2
            null,                                                                                   // dd d3
            null,                                                                                   // dd d4
            null,                                                                                   // dd d5
            null,                                                                                   // dd d6
            null,                                                                                   // dd d7
            null,                                                                                   // dd d8
            null,                                                                                   // dd d9
            null,                                                                                   // dd da
            null,                                                                                   // dd db
            null,                                                                                   // dd dc
            null,                                                                                   // dd dd
            null,                                                                                   // dd de
            null,                                                                                   // dd df
            null,                                                                                   // dd e0
            new Opcode(){ Extension = null, DisasmFormat = "pop ix",                    Size = 2 }, // dd e1
            null,                                                                                   // dd e2
            new Opcode(){ Extension = null, DisasmFormat = "ex (sp),ix",                Size = 2 }, // dd e3
            null,                                                                                   // dd e4
            new Opcode(){ Extension = null, DisasmFormat = "push ix",                   Size = 2 }, // dd e5
            null,                                                                                   // dd e6
            null,                                                                                   // dd e7
            null,                                                                                   // dd e8
            new Opcode(){ Extension = null, DisasmFormat = "jp (ix)",                   Size = 2 }, // dd e9
            null,                                                                                   // dd ea
            null,                                                                                   // dd eb
            null,                                                                                   // dd ec
            null,                                                                                   // dd ed
            null,                                                                                   // dd ee
            null,                                                                                   // dd ef
            null,                                                                                   // dd f0
            null,                                                                                   // dd f1
            null,                                                                                   // dd f2
            null,                                                                                   // dd f3
            null,                                                                                   // dd f4
            null,                                                                                   // dd f5
            null,                                                                                   // dd f6
            null,                                                                                   // dd f7
            null,                                                                                   // dd f8
            new Opcode(){ Extension = null, DisasmFormat = "ld sp,ix",                  Size = 2 }, // dd f9
            null,                                                                                   // dd fa
            null,                                                                                   // dd fb
            null,                                                                                   // dd fc
            null,                                                                                   // dd fd
            null,                                                                                   // dd fe
            null,                                                                                   // dd ff
        };

        private Opcode[] _iyOpcodes = 
        {
            null,                                                                                   // fd 00
            null,                                                                                   // fd 01
            null,                                                                                   // fd 02
            null,                                                                                   // fd 03
            null,                                                                                   // fd 04
            null,                                                                                   // fd 05
            null,                                                                                   // fd 06
            null,                                                                                   // fd 07
            null,                                                                                   // fd 08
            new Opcode(){ Extension = null, DisasmFormat = "add iy,bc",                 Size = 2 }, // fd 09
            null,                                                                                   // fd 0a
            null,                                                                                   // fd 0b
            null,                                                                                   // fd 0c
            null,                                                                                   // fd 0d
            null,                                                                                   // fd 0e
            null,                                                                                   // fd 0f
            null,                                                                                   // fd 10
            null,                                                                                   // fd 11
            null,                                                                                   // fd 12
            null,                                                                                   // fd 13
            null,                                                                                   // fd 14
            null,                                                                                   // fd 15
            null,                                                                                   // fd 16
            null,                                                                                   // fd 17
            null,                                                                                   // fd 18
            new Opcode(){ Extension = null, DisasmFormat = "add iy,de",                 Size = 2 }, // fd 19
            null,                                                                                   // fd 1a
            null,                                                                                   // fd 1b
            null,                                                                                   // fd 1c
            null,                                                                                   // fd 1d
            null,                                                                                   // fd 1e
            null,                                                                                   // fd 1f
            null,                                                                                   // fd 20
            new Opcode(){ Extension = null, DisasmFormat = "ld iy,${0:x4}",             Size = 4 }, // fd 21
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),iy",           Size = 4 }, // fd 22
            new Opcode(){ Extension = null, DisasmFormat = "inc iy",                    Size = 2 }, // fd 23
            new Opcode(){ Extension = null, DisasmFormat = "inc iyh",                   Size = 2 }, // fd 24
            new Opcode(){ Extension = null, DisasmFormat = "dec iyh",                   Size = 2 }, // fd 25
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,${0:x2}",            Size = 3 }, // fd 26
            null,                                                                                   // fd 27
            null,                                                                                   // fd 28
            new Opcode(){ Extension = null, DisasmFormat = "add iy,iy",                 Size = 2 }, // fd 29
            new Opcode(){ Extension = null, DisasmFormat = "ld iy,(${0:x4})",           Size = 4 }, // fd 2a
            new Opcode(){ Extension = null, DisasmFormat = "dec iy",                    Size = 2 }, // fd 2b
            new Opcode(){ Extension = null, DisasmFormat = "inc iyl",                   Size = 2 }, // fd 2c
            new Opcode(){ Extension = null, DisasmFormat = "dec iyl",                   Size = 2 }, // fd 2d
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,${0:x2}",            Size = 3 }, // fd 2e
            null,                                                                                   // fd 2f
            null,                                                                                   // fd 30
            null,                                                                                   // fd 31
            null,                                                                                   // fd 32
            null,                                                                                   // fd 33
            new Opcode(){ Extension = null, DisasmFormat = "inc (iy+${0:x2})",          Size = 3 }, // fd 34
            new Opcode(){ Extension = null, DisasmFormat = "dec (iy+${0:x2})",          Size = 3 }, // fd 35
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),${1:x2}",   Size = 4 }, // fd 36
            null,                                                                                   // fd 37
            null,                                                                                   // fd 38
            new Opcode(){ Extension = null, DisasmFormat = "add iy,sp",                 Size = 2 }, // fd 39
            null,                                                                                   // fd 3a
            null,                                                                                   // fd 3b
            null,                                                                                   // fd 3c
            null,                                                                                   // fd 3d
            null,                                                                                   // fd 3e
            null,                                                                                   // fd 3f
            null,                                                                                   // fd 40
            null,                                                                                   // fd 41
            null,                                                                                   // fd 42
            null,                                                                                   // fd 43
            new Opcode(){ Extension = null, DisasmFormat = "ld b,iyh",                  Size = 2 }, // fd 44
            new Opcode(){ Extension = null, DisasmFormat = "ld b,iyl",                  Size = 2 }, // fd 45
            new Opcode(){ Extension = null, DisasmFormat = "ld b,(iy+${0:x2})",         Size = 3 }, // fd 46
            null,                                                                                   // fd 47
            null,                                                                                   // fd 48
            null,                                                                                   // fd 49
            null,                                                                                   // fd 4a
            null,                                                                                   // fd 4b
            new Opcode(){ Extension = null, DisasmFormat = "ld c,iyh",                  Size = 2 }, // fd 4c
            new Opcode(){ Extension = null, DisasmFormat = "ld c,iyl",                  Size = 2 }, // fd 4d
            new Opcode(){ Extension = null, DisasmFormat = "ld c,(iy+${0:x2})",         Size = 3 }, // fd 4e
            null,                                                                                   // fd 4f
            null,                                                                                   // fd 50
            null,                                                                                   // fd 51
            null,                                                                                   // fd 52
            null,                                                                                   // fd 53
            new Opcode(){ Extension = null, DisasmFormat = "ld d,iyh",                  Size = 2 }, // fd 54
            new Opcode(){ Extension = null, DisasmFormat = "ld d,iyl",                  Size = 2 }, // fd 55
            new Opcode(){ Extension = null, DisasmFormat = "ld d,(iy+${0:x2})",         Size = 3 }, // fd 56
            null,                                                                                   // fd 57
            null,                                                                                   // fd 58
            null,                                                                                   // fd 59
            null,                                                                                   // fd 5a
            null,                                                                                   // fd 5b
            new Opcode(){ Extension = null, DisasmFormat = "ld e,iyh",                  Size = 2 }, // fd 5c
            new Opcode(){ Extension = null, DisasmFormat = "ld e,iyl",                  Size = 2 }, // fd 5d
            new Opcode(){ Extension = null, DisasmFormat = "ld e,(iy+${0:x2})",         Size = 3 }, // fd 5e
            null,                                                                                   // fd 5f
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,b",                  Size = 2 }, // fd 60
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,c",                  Size = 2 }, // fd 61
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,d",                  Size = 2 }, // fd 62
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,e",                  Size = 2 }, // fd 63
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,iyh",                  Size = 2 }, // fd 64
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,iyl",                  Size = 2 }, // fd 65
            new Opcode(){ Extension = null, DisasmFormat = "ld h,(iy+${0:x2})",         Size = 3 }, // fd 66
            new Opcode(){ Extension = null, DisasmFormat = "ld iyh,a",                  Size = 2 }, // fd 67
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,b",                  Size = 2 }, // fd 68
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,c",                  Size = 2 }, // fd 69
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,d",                  Size = 2 }, // fd 6a
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,e",                  Size = 2 }, // fd 6b
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,iyh",                Size = 2 }, // fd 6c
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,iyl",                Size = 2 }, // fd 6d
            new Opcode(){ Extension = null, DisasmFormat = "ld l,(iy+${0:x2})",         Size = 3 }, // fd 6e
            new Opcode(){ Extension = null, DisasmFormat = "ld iyl,a",                  Size = 2 }, // fd 6f
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),b",         Size = 3 }, // fd 70
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),c",         Size = 3 }, // fd 71
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),d",         Size = 3 }, // fd 72
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),e",         Size = 3 }, // fd 73
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),h",         Size = 3 }, // fd 74
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),l",         Size = 3 }, // fd 75
            null,                                                                                   // fd 76
            new Opcode(){ Extension = null, DisasmFormat = "ld (iy+${0:x2}),a",         Size = 3 }, // fd 77
            null,                                                                                   // fd 78
            null,                                                                                   // fd 79
            null,                                                                                   // fd 7a
            null,                                                                                   // fd 7b
            new Opcode(){ Extension = null, DisasmFormat = "ld a,iyh",                  Size = 2 }, // fd 7c
            new Opcode(){ Extension = null, DisasmFormat = "ld a,iyl",                  Size = 2 }, // fd 7d
            new Opcode(){ Extension = null, DisasmFormat = "ld a,(iy+${0:x2})",         Size = 3 }, // fd 7e
            null,                                                                                   // fd 7f
            null,                                                                                   // fd 80
            null,                                                                                   // fd 81
            null,                                                                                   // fd 82
            null,                                                                                   // fd 83
            new Opcode(){ Extension = null, DisasmFormat = "add a,iyh",                 Size = 2 }, // fd 84
            new Opcode(){ Extension = null, DisasmFormat = "add a,iyl",                 Size = 2 }, // fd 85
            new Opcode(){ Extension = null, DisasmFormat = "add a,(iy+${0:x2})",        Size = 3 }, // fd 86
            null,                                                                                   // fd 87
            null,                                                                                   // fd 88
            null,                                                                                   // fd 89
            null,                                                                                   // fd 8a
            null,                                                                                   // fd 8b
            new Opcode(){ Extension = null, DisasmFormat = "adc a,iyh",                 Size = 2 }, // fd 8c
            new Opcode(){ Extension = null, DisasmFormat = "adc a,iyl",                 Size = 2 }, // fd 8d
            new Opcode(){ Extension = null, DisasmFormat = "adc a,(iy+${0:x2})",        Size = 3 }, // fd 8e
            null,                                                                                   // fd 8f
            null,                                                                                   // fd 90
            null,                                                                                   // fd 91
            null,                                                                                   // fd 92
            null,                                                                                   // fd 93
            new Opcode(){ Extension = null, DisasmFormat = "sub iyh",                   Size = 2 }, // fd 94
            new Opcode(){ Extension = null, DisasmFormat = "sub iyl",                   Size = 2 }, // fd 95
            new Opcode(){ Extension = null, DisasmFormat = "sub (iy+${0:x2})",          Size = 3 }, // fd 96
            null,                                                                                   // fd 97
            null,                                                                                   // fd 98
            null,                                                                                   // fd 99
            null,                                                                                   // fd 9a
            null,                                                                                   // fd 9b
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,iyh",                 Size = 2 }, // fd 9c
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,iyl",                 Size = 2 }, // fd 9d
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,(iy+${0:x2})",        Size = 3 }, // fd 9e
            null,                                                                                   // fd 9f
            null,                                                                                   // fd a0
            null,                                                                                   // fd a1
            null,                                                                                   // fd a2
            null,                                                                                   // fd a3
            new Opcode(){ Extension = null, DisasmFormat = "and iyh",                   Size = 2 }, // fd a4
            new Opcode(){ Extension = null, DisasmFormat = "and iyl",                   Size = 2 }, // fd a5
            new Opcode(){ Extension = null, DisasmFormat = "and (iy+${0:x2})",          Size = 3 }, // fd a6
            null,                                                                                   // fd a7
            null,                                                                                   // fd a8
            null,                                                                                   // fd a9
            null,                                                                                   // fd aa
            null,                                                                                   // fd ab
            new Opcode(){ Extension = null, DisasmFormat = "xor iyh",                   Size = 2 }, // fd ac
            new Opcode(){ Extension = null, DisasmFormat = "xor iyl",                   Size = 2 }, // fd ad
            new Opcode(){ Extension = null, DisasmFormat = "xor (iy+${0:x2})",          Size = 3 }, // fd ae            
            null,                                                                                   // fd af
            null,                                                                                   // fd b0
            null,                                                                                   // fd b1
            null,                                                                                   // fd b2
            null,                                                                                   // fd b3
            new Opcode(){ Extension = null, DisasmFormat = "or iyh",                    Size = 2 }, // fd b4
            new Opcode(){ Extension = null, DisasmFormat = "or iyl",                    Size = 2 }, // fd b5
            new Opcode(){ Extension = null, DisasmFormat = "or (iy+${0:x2})",           Size = 3 }, // fd b6
            null,                                                                                   // fd b7
            null,                                                                                   // fd b8
            null,                                                                                   // fd b9
            null,                                                                                   // fd ba
            null,                                                                                   // fd bb
            new Opcode(){ Extension = null, DisasmFormat = "cp iyh",                    Size = 2 }, // fd bc
            new Opcode(){ Extension = null, DisasmFormat = "cp iyl",                    Size = 2 }, // fd bd
            new Opcode(){ Extension = null, DisasmFormat = "cp (iy+${0:x2})",           Size = 3 }, // fd be
            null,                                                                                   // fd bf
            null,                                                                                   // fd c0
            null,                                                                                   // fd c1
            null,                                                                                   // fd c2
            null,                                                                                   // fd c3
            null,                                                                                   // fd c4
            null,                                                                                   // fd c5
            null,                                                                                   // fd c6
            null,                                                                                   // fd c7
            null,                                                                                   // fd c8
            null,                                                                                   // fd c9
            null,                                                                                   // fd ca
            new Opcode(){ Extension = null, DisasmFormat = null,                        Size = 3 }, // fd cb
            null,                                                                                   // fd cc
            null,                                                                                   // fd cd
            null,                                                                                   // fd ce
            null,                                                                                   // fd cf
            null,                                                                                   // fd d0
            null,                                                                                   // fd d1
            null,                                                                                   // fd d2
            null,                                                                                   // fd d3
            null,                                                                                   // fd d4
            null,                                                                                   // fd d5
            null,                                                                                   // fd d6
            null,                                                                                   // fd d7
            null,                                                                                   // fd d8
            null,                                                                                   // fd d9
            null,                                                                                   // fd da
            null,                                                                                   // fd db
            null,                                                                                   // fd dc
            null,                                                                                   // fd dd
            null,                                                                                   // fd de
            null,                                                                                   // fd df
            null,                                                                                   // fd e0
            new Opcode(){ Extension = null, DisasmFormat = "pop iy",                    Size = 2 }, // fd e1
            null,                                                                                   // fd e2
            new Opcode(){ Extension = null, DisasmFormat = "ex (sp),iy",                Size = 2 }, // fd e3
            null,                                                                                   // fd e4
            new Opcode(){ Extension = null, DisasmFormat = "push iy",                   Size = 2 }, // fd e5
            null,                                                                                   // fd e6
            null,                                                                                   // fd e7
            null,                                                                                   // fd e8
            new Opcode(){ Extension = null, DisasmFormat = "jp (iy)",                   Size = 2 }, // fd e9
            null,                                                                                   // fd ea
            null,                                                                                   // fd eb
            null,                                                                                   // fd ec
            null,                                                                                   // fd ed
            null,                                                                                   // fd ee
            null,                                                                                   // fd ef
            null,                                                                                   // fd f0
            null,                                                                                   // fd f1
            null,                                                                                   // fd f2
            null,                                                                                   // fd f3
            null,                                                                                   // fd f4
            null,                                                                                   // fd f5
            null,                                                                                   // fd f6
            null,                                                                                   // fd f7
            null,                                                                                   // fd f8
            new Opcode(){ Extension = null, DisasmFormat = "ld sp,iy",                  Size = 2 }, // fd f9
            null,                                                                                   // fd fa
            null,                                                                                   // fd fb
            null,                                                                                   // fd fc
            null,                                                                                   // fd fd
            null,                                                                                   // fd fe
            null,                                                                                   // fd ff
        };

        private Opcode[] _opcodes = 
        {
            new Opcode(){ Extension = null, DisasmFormat = "nop",               Size = 1 }, // 00
            new Opcode(){ Extension = null, DisasmFormat = "ld bc,${0:x4}",     Size = 3 }, // 01
            new Opcode(){ Extension = null, DisasmFormat = "ld (bc),a",         Size = 1 }, // 02
            new Opcode(){ Extension = null, DisasmFormat = "inc bc",            Size = 1 }, // 03
            new Opcode(){ Extension = null, DisasmFormat = "inc b",             Size = 1 }, // 04
            new Opcode(){ Extension = null, DisasmFormat = "dec b",             Size = 1 }, // 05
            new Opcode(){ Extension = null, DisasmFormat = "ld b,${0:x2}",      Size = 2 }, // 06
            new Opcode(){ Extension = null, DisasmFormat = "rlca",              Size = 1 }, // 07
            new Opcode(){ Extension = null, DisasmFormat = "ex af,af'",         Size = 1 }, // 08
            new Opcode(){ Extension = null, DisasmFormat = "add hl,bc",         Size = 1 }, // 09
            new Opcode(){ Extension = null, DisasmFormat = "ld a,(bc)",         Size = 1 }, // 0a
            new Opcode(){ Extension = null, DisasmFormat = "dec bc",            Size = 1 }, // 0b
            new Opcode(){ Extension = null, DisasmFormat = "inc c",             Size = 1 }, // 0c
            new Opcode(){ Extension = null, DisasmFormat = "dec c",             Size = 1 }, // 0d
            new Opcode(){ Extension = null, DisasmFormat = "ld c,${0:x2}",      Size = 2 }, // 0e
            new Opcode(){ Extension = null, DisasmFormat = "rrca",              Size = 1 }, // 0f
            new Opcode(){ Extension = null, DisasmFormat = "djnz ${0:x4}",      Size = 2 }, // 10
            new Opcode(){ Extension = null, DisasmFormat = "ld de,${0:x4}",     Size = 3 }, // 11
            new Opcode(){ Extension = null, DisasmFormat = "ld (de),a",         Size = 1 }, // 12
            new Opcode(){ Extension = null, DisasmFormat = "inc de",            Size = 1 }, // 13
            new Opcode(){ Extension = null, DisasmFormat = "inc d",             Size = 1 }, // 14
            new Opcode(){ Extension = null, DisasmFormat = "dec d",             Size = 1 }, // 15
            new Opcode(){ Extension = null, DisasmFormat = "ld d,${0:x2}",      Size = 2 }, // 16
            new Opcode(){ Extension = null, DisasmFormat = "rla",               Size = 1 }, // 17
            new Opcode(){ Extension = null, DisasmFormat = "jr ${0:x4}",        Size = 2 }, // 18
            new Opcode(){ Extension = null, DisasmFormat = "add hl,de",         Size = 1 }, // 19
            new Opcode(){ Extension = null, DisasmFormat = "ld a,(de)",         Size = 1 }, // 1a
            new Opcode(){ Extension = null, DisasmFormat = "dec de",            Size = 1 }, // 1b
            new Opcode(){ Extension = null, DisasmFormat = "inc e",             Size = 1 }, // 1c
            new Opcode(){ Extension = null, DisasmFormat = "dec e",             Size = 1 }, // 1d
            new Opcode(){ Extension = null, DisasmFormat = "ld e,${0:x2}",      Size = 2 }, // 1e
            new Opcode(){ Extension = null, DisasmFormat = "rra",               Size = 1 }, // 1f
            new Opcode(){ Extension = null, DisasmFormat = "jr nz,${0:x4}",     Size = 2 }, // 20
            new Opcode(){ Extension = null, DisasmFormat = "ld hl,${0:x4}",     Size = 3 }, // 21
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),hl",   Size = 3 }, // 22
            new Opcode(){ Extension = null, DisasmFormat = "inc hl",            Size = 1 }, // 23
            new Opcode(){ Extension = null, DisasmFormat = "inc h",             Size = 1 }, // 24
            new Opcode(){ Extension = null, DisasmFormat = "dec h",             Size = 1 }, // 25
            new Opcode(){ Extension = null, DisasmFormat = "ld h,${0:x2}",      Size = 2 }, // 26
            new Opcode(){ Extension = null, DisasmFormat = "daa",               Size = 1 }, // 27
            new Opcode(){ Extension = null, DisasmFormat = "jr z,${0:x4}",      Size = 2 }, // 28
            new Opcode(){ Extension = null, DisasmFormat = "add hl,hl",         Size = 1 }, // 29
            new Opcode(){ Extension = null, DisasmFormat = "ld hl,(${0:x4})",   Size = 3 }, // 2a
            new Opcode(){ Extension = null, DisasmFormat = "dec hl",            Size = 1 }, // 2b
            new Opcode(){ Extension = null, DisasmFormat = "inc l",             Size = 1 }, // 2c
            new Opcode(){ Extension = null, DisasmFormat = "dec l",             Size = 1 }, // 2d
            new Opcode(){ Extension = null, DisasmFormat = "ld l,${0:x2}",      Size = 2 }, // 2e
            new Opcode(){ Extension = null, DisasmFormat = "cpl",               Size = 1 }, // 2f
            new Opcode(){ Extension = null, DisasmFormat = "jr nc,${0:x4}",     Size = 2 }, // 30
            new Opcode(){ Extension = null, DisasmFormat = "ld sp,${0:x4}",     Size = 3 }, // 31
            new Opcode(){ Extension = null, DisasmFormat = "ld (${0:x4}),a",    Size = 3 }, // 32
            new Opcode(){ Extension = null, DisasmFormat = "inc sp",            Size = 1 }, // 33
            new Opcode(){ Extension = null, DisasmFormat = "inc (hl)",          Size = 1 }, // 34
            new Opcode(){ Extension = null, DisasmFormat = "dec (hl)",          Size = 1 }, // 35
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),${0:x2}",   Size = 2 }, // 36
            new Opcode(){ Extension = null, DisasmFormat = "scf",               Size = 1 }, // 37
            new Opcode(){ Extension = null, DisasmFormat = "jr c,${0:x4}",      Size = 2 }, // 38
            new Opcode(){ Extension = null, DisasmFormat = "add hl,sp",         Size = 1 }, // 39
            new Opcode(){ Extension = null, DisasmFormat = "ld a,(${0:x4})",    Size = 3 }, // 3a
            new Opcode(){ Extension = null, DisasmFormat = "dec sp",            Size = 1 }, // 3b
            new Opcode(){ Extension = null, DisasmFormat = "inc a",             Size = 1 }, // 3c
            new Opcode(){ Extension = null, DisasmFormat = "dec a",             Size = 1 }, // 3d
            new Opcode(){ Extension = null, DisasmFormat = "ld a,${0:x2}",      Size = 2 }, // 3e
            new Opcode(){ Extension = null, DisasmFormat = "ccf",               Size = 1 }, // 3f
            new Opcode(){ Extension = null, DisasmFormat = "ld b,b",            Size = 1 }, // 40
            new Opcode(){ Extension = null, DisasmFormat = "ld b,c",            Size = 1 }, // 41
            new Opcode(){ Extension = null, DisasmFormat = "ld b,d",            Size = 1 }, // 42
            new Opcode(){ Extension = null, DisasmFormat = "ld b,e",            Size = 1 }, // 43
            new Opcode(){ Extension = null, DisasmFormat = "ld b,h",            Size = 1 }, // 44
            new Opcode(){ Extension = null, DisasmFormat = "ld b,l",            Size = 1 }, // 45
            new Opcode(){ Extension = null, DisasmFormat = "ld b,(hl)",         Size = 1 }, // 46
            new Opcode(){ Extension = null, DisasmFormat = "ld b,a",            Size = 1 }, // 47
            new Opcode(){ Extension = null, DisasmFormat = "ld c,b",            Size = 1 }, // 48
            new Opcode(){ Extension = null, DisasmFormat = "ld c,c",            Size = 1 }, // 49
            new Opcode(){ Extension = null, DisasmFormat = "ld c,d",            Size = 1 }, // 4a
            new Opcode(){ Extension = null, DisasmFormat = "ld c,e",            Size = 1 }, // 4b
            new Opcode(){ Extension = null, DisasmFormat = "ld c,h",            Size = 1 }, // 4c
            new Opcode(){ Extension = null, DisasmFormat = "ld c,l",            Size = 1 }, // 4d
            new Opcode(){ Extension = null, DisasmFormat = "ld c,(hl)",         Size = 1 }, // 4e
            new Opcode(){ Extension = null, DisasmFormat = "ld c,a",            Size = 1 }, // 4f
            new Opcode(){ Extension = null, DisasmFormat = "ld d,b",            Size = 1 }, // 50
            new Opcode(){ Extension = null, DisasmFormat = "ld d,c",            Size = 1 }, // 51
            new Opcode(){ Extension = null, DisasmFormat = "ld d,d",            Size = 1 }, // 52
            new Opcode(){ Extension = null, DisasmFormat = "ld d,e",            Size = 1 }, // 53
            new Opcode(){ Extension = null, DisasmFormat = "ld d,h",            Size = 1 }, // 54
            new Opcode(){ Extension = null, DisasmFormat = "ld d,l",            Size = 1 }, // 55
            new Opcode(){ Extension = null, DisasmFormat = "ld d,(hl)",         Size = 1 }, // 56
            new Opcode(){ Extension = null, DisasmFormat = "ld d,a",            Size = 1 }, // 57
            new Opcode(){ Extension = null, DisasmFormat = "ld e,b",            Size = 1 }, // 58
            new Opcode(){ Extension = null, DisasmFormat = "ld e,c",            Size = 1 }, // 59
            new Opcode(){ Extension = null, DisasmFormat = "ld e,d",            Size = 1 }, // 5a
            new Opcode(){ Extension = null, DisasmFormat = "ld e,e",            Size = 1 }, // 5b
            new Opcode(){ Extension = null, DisasmFormat = "ld e,h",            Size = 1 }, // 5c
            new Opcode(){ Extension = null, DisasmFormat = "ld e,l",            Size = 1 }, // 5d
            new Opcode(){ Extension = null, DisasmFormat = "ld e,(hl)",         Size = 1 }, // 5e
            new Opcode(){ Extension = null, DisasmFormat = "ld e,a",            Size = 1 }, // 5f
            new Opcode(){ Extension = null, DisasmFormat = "ld h,b",            Size = 1 }, // 60
            new Opcode(){ Extension = null, DisasmFormat = "ld h,c",            Size = 1 }, // 61
            new Opcode(){ Extension = null, DisasmFormat = "ld h,d",            Size = 1 }, // 62
            new Opcode(){ Extension = null, DisasmFormat = "ld h,e",            Size = 1 }, // 63
            new Opcode(){ Extension = null, DisasmFormat = "ld h,h",            Size = 1 }, // 64
            new Opcode(){ Extension = null, DisasmFormat = "ld h,l",            Size = 1 }, // 65
            new Opcode(){ Extension = null, DisasmFormat = "ld h,(hl)",         Size = 1 }, // 66
            new Opcode(){ Extension = null, DisasmFormat = "ld h,a",            Size = 1 }, // 67
            new Opcode(){ Extension = null, DisasmFormat = "ld l,b",            Size = 1 }, // 68
            new Opcode(){ Extension = null, DisasmFormat = "ld l,c",            Size = 1 }, // 69
            new Opcode(){ Extension = null, DisasmFormat = "ld l,d",            Size = 1 }, // 6a
            new Opcode(){ Extension = null, DisasmFormat = "ld l,e",            Size = 1 }, // 6b
            new Opcode(){ Extension = null, DisasmFormat = "ld l,h",            Size = 1 }, // 6c
            new Opcode(){ Extension = null, DisasmFormat = "ld l,l",            Size = 1 }, // 6d
            new Opcode(){ Extension = null, DisasmFormat = "ld l,(hl)",         Size = 1 }, // 6e
            new Opcode(){ Extension = null, DisasmFormat = "ld l,a",            Size = 1 }, // 6f
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),b",         Size = 1 }, // 70
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),c",         Size = 1 }, // 71
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),d",         Size = 1 }, // 72
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),e",         Size = 1 }, // 73
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),h",         Size = 1 }, // 74
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),l",         Size = 1 }, // 75
            new Opcode(){ Extension = null, DisasmFormat = "halt",              Size = 1 }, // 76
            new Opcode(){ Extension = null, DisasmFormat = "ld (hl),a",         Size = 1 }, // 77
            new Opcode(){ Extension = null, DisasmFormat = "ld a,b",            Size = 1 }, // 78
            new Opcode(){ Extension = null, DisasmFormat = "ld a,c",            Size = 1 }, // 79
            new Opcode(){ Extension = null, DisasmFormat = "ld a,d",            Size = 1 }, // 7a
            new Opcode(){ Extension = null, DisasmFormat = "ld a,e",            Size = 1 }, // 7b
            new Opcode(){ Extension = null, DisasmFormat = "ld a,h",            Size = 1 }, // 7c
            new Opcode(){ Extension = null, DisasmFormat = "ld a,l",            Size = 1 }, // 7d
            new Opcode(){ Extension = null, DisasmFormat = "ld a,(hl)",         Size = 1 }, // 7e
            new Opcode(){ Extension = null, DisasmFormat = "ld a,a",            Size = 1 }, // 7f
            new Opcode(){ Extension = null, DisasmFormat = "add a,b",           Size = 1 }, // 80
            new Opcode(){ Extension = null, DisasmFormat = "add a,c",           Size = 1 }, // 81
            new Opcode(){ Extension = null, DisasmFormat = "add a,d",           Size = 1 }, // 82
            new Opcode(){ Extension = null, DisasmFormat = "add a,e",           Size = 1 }, // 83
            new Opcode(){ Extension = null, DisasmFormat = "add a,h",           Size = 1 }, // 84
            new Opcode(){ Extension = null, DisasmFormat = "add a,l",           Size = 1 }, // 85
            new Opcode(){ Extension = null, DisasmFormat = "add a,(hl)",        Size = 1 }, // 86
            new Opcode(){ Extension = null, DisasmFormat = "add a,a",           Size = 1 }, // 87
            new Opcode(){ Extension = null, DisasmFormat = "adc a,b",           Size = 1 }, // 88
            new Opcode(){ Extension = null, DisasmFormat = "adc a,c",           Size = 1 }, // 89
            new Opcode(){ Extension = null, DisasmFormat = "adc a,d",           Size = 1 }, // 8a
            new Opcode(){ Extension = null, DisasmFormat = "adc a,e",           Size = 1 }, // 8b
            new Opcode(){ Extension = null, DisasmFormat = "adc a,h",           Size = 1 }, // 8c
            new Opcode(){ Extension = null, DisasmFormat = "adc a,l",           Size = 1 }, // 8d
            new Opcode(){ Extension = null, DisasmFormat = "adc a,(hl)",        Size = 1 }, // 8e
            new Opcode(){ Extension = null, DisasmFormat = "adc a,a",           Size = 1 }, // 8f
            new Opcode(){ Extension = null, DisasmFormat = "sub b",             Size = 1 }, // 90
            new Opcode(){ Extension = null, DisasmFormat = "sub c",             Size = 1 }, // 91
            new Opcode(){ Extension = null, DisasmFormat = "sub d",             Size = 1 }, // 92
            new Opcode(){ Extension = null, DisasmFormat = "sub e",             Size = 1 }, // 93
            new Opcode(){ Extension = null, DisasmFormat = "sub h",             Size = 1 }, // 94
            new Opcode(){ Extension = null, DisasmFormat = "sub l",             Size = 1 }, // 95
            new Opcode(){ Extension = null, DisasmFormat = "sub (hl)",          Size = 1 }, // 96
            new Opcode(){ Extension = null, DisasmFormat = "sub a",             Size = 1 }, // 97
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,b",           Size = 1 }, // 98
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,c",           Size = 1 }, // 99
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,d",           Size = 1 }, // 9a
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,e",           Size = 1 }, // 9b
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,h",           Size = 1 }, // 9c
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,l",           Size = 1 }, // 9d
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,(hl)",        Size = 1 }, // 9e
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,a",           Size = 1 }, // 9f
            new Opcode(){ Extension = null, DisasmFormat = "and b",             Size = 1 }, // a0
            new Opcode(){ Extension = null, DisasmFormat = "and c",             Size = 1 }, // a1
            new Opcode(){ Extension = null, DisasmFormat = "and d",             Size = 1 }, // a2
            new Opcode(){ Extension = null, DisasmFormat = "and e",             Size = 1 }, // a3
            new Opcode(){ Extension = null, DisasmFormat = "and h",             Size = 1 }, // a4
            new Opcode(){ Extension = null, DisasmFormat = "and l",             Size = 1 }, // a5
            new Opcode(){ Extension = null, DisasmFormat = "and (hl)",          Size = 1 }, // a6
            new Opcode(){ Extension = null, DisasmFormat = "and a",             Size = 1 }, // a7
            new Opcode(){ Extension = null, DisasmFormat = "xor b",             Size = 1 }, // a8
            new Opcode(){ Extension = null, DisasmFormat = "xor c",             Size = 1 }, // a9
            new Opcode(){ Extension = null, DisasmFormat = "xor d",             Size = 1 }, // aa
            new Opcode(){ Extension = null, DisasmFormat = "xor e",             Size = 1 }, // ab
            new Opcode(){ Extension = null, DisasmFormat = "xor h",             Size = 1 }, // ac
            new Opcode(){ Extension = null, DisasmFormat = "xor l",             Size = 1 }, // ad
            new Opcode(){ Extension = null, DisasmFormat = "xor (hl)",          Size = 1 }, // ae
            new Opcode(){ Extension = null, DisasmFormat = "xor a",             Size = 1 }, // af
            new Opcode(){ Extension = null, DisasmFormat = "or b",              Size = 1 }, // b0
            new Opcode(){ Extension = null, DisasmFormat = "or c",              Size = 1 }, // b1
            new Opcode(){ Extension = null, DisasmFormat = "or d",              Size = 1 }, // b2
            new Opcode(){ Extension = null, DisasmFormat = "or e",              Size = 1 }, // b3
            new Opcode(){ Extension = null, DisasmFormat = "or h",              Size = 1 }, // b4
            new Opcode(){ Extension = null, DisasmFormat = "or l",              Size = 1 }, // b5
            new Opcode(){ Extension = null, DisasmFormat = "or (hl)",           Size = 1 }, // b6
            new Opcode(){ Extension = null, DisasmFormat = "or a",              Size = 1 }, // b7
            new Opcode(){ Extension = null, DisasmFormat = "cp b",              Size = 1 }, // b8
            new Opcode(){ Extension = null, DisasmFormat = "cp c",              Size = 1 }, // b9
            new Opcode(){ Extension = null, DisasmFormat = "cp d",              Size = 1 }, // ba
            new Opcode(){ Extension = null, DisasmFormat = "cp e",              Size = 1 }, // bb
            new Opcode(){ Extension = null, DisasmFormat = "cp h",              Size = 1 }, // bc
            new Opcode(){ Extension = null, DisasmFormat = "cp l",              Size = 1 }, // bd
            new Opcode(){ Extension = null, DisasmFormat = "cp (hl)",           Size = 1 }, // be
            new Opcode(){ Extension = null, DisasmFormat = "cp a",              Size = 1 }, // bf
            new Opcode(){ Extension = null, DisasmFormat = "ret nz",            Size = 1 }, // c0
            new Opcode(){ Extension = null, DisasmFormat = "pop bc",            Size = 1 }, // c1
            new Opcode(){ Extension = null, DisasmFormat = "jp nz,${0:x4}",     Size = 3 }, // c2
            new Opcode(){ Extension = null, DisasmFormat = "jp ${0:x4}",        Size = 3 }, // c3
            new Opcode(){ Extension = null, DisasmFormat = "call nz,${0:x4}",   Size = 3 }, // c4
            new Opcode(){ Extension = null, DisasmFormat = "push bc",           Size = 1 }, // c5
            new Opcode(){ Extension = null, DisasmFormat = "add a,${0:x2}",     Size = 2 }, // c6
            new Opcode(){ Extension = null, DisasmFormat = "rst $00",           Size = 1 }, // c7
            new Opcode(){ Extension = null, DisasmFormat = "ret z",             Size = 1 }, // c8
            new Opcode(){ Extension = null, DisasmFormat = "ret",               Size = 1 }, // c9
            new Opcode(){ Extension = null, DisasmFormat = "jp z,${0:x4}",      Size = 3 }, // ca
            new Opcode(){ Extension = null, DisasmFormat = null,                Size = 2 }, // cb
            new Opcode(){ Extension = null, DisasmFormat = "call z,${0:x4}",    Size = 3 }, // cc
            new Opcode(){ Extension = null, DisasmFormat = "call ${0:x4}",      Size = 3 }, // cd
            new Opcode(){ Extension = null, DisasmFormat = "adc a,${0:x2}",     Size = 2 }, // ce
            new Opcode(){ Extension = null, DisasmFormat = "rst $08",           Size = 1 }, // cf
            new Opcode(){ Extension = null, DisasmFormat = "ret nc",            Size = 1 }, // d0
            new Opcode(){ Extension = null, DisasmFormat = "pop de",            Size = 1 }, // d1
            new Opcode(){ Extension = null, DisasmFormat = "jp nc,${0:x4}",     Size = 3 }, // d2
            new Opcode(){ Extension = null, DisasmFormat = "out (${0:x2}),a",   Size = 2 }, // d3
            new Opcode(){ Extension = null, DisasmFormat = "call nc,${0:x4}",   Size = 3 }, // d4
            new Opcode(){ Extension = null, DisasmFormat = "push de",           Size = 1 }, // d5
            new Opcode(){ Extension = null, DisasmFormat = "sub ${0:x2}",       Size = 2 }, // d6
            new Opcode(){ Extension = null, DisasmFormat = "rst $10",           Size = 1 }, // d7
            new Opcode(){ Extension = null, DisasmFormat = "ret c",             Size = 1 }, // d8
            new Opcode(){ Extension = null, DisasmFormat = "exx",               Size = 1 }, // d9
            new Opcode(){ Extension = null, DisasmFormat = "jp c,${0:x4}",      Size = 3 }, // da
            new Opcode(){ Extension = null, DisasmFormat = "in a,(${0:x2})",    Size = 2 }, // db
            new Opcode(){ Extension = null, DisasmFormat = "call c,${0:x4}",    Size = 3 }, // dc
            new Opcode(){ Extension = null, DisasmFormat = null,                Size = 2 }, // dd
            new Opcode(){ Extension = null, DisasmFormat = "sbc a,${0:x2}",     Size = 2 }, // de
            new Opcode(){ Extension = null, DisasmFormat = "rst $18",           Size = 1 }, // df
            new Opcode(){ Extension = null, DisasmFormat = "ret po",            Size = 1 }, // e0
            new Opcode(){ Extension = null, DisasmFormat = "pop hl",            Size = 1 }, // e1
            new Opcode(){ Extension = null, DisasmFormat = "jp po,${0:x4}",     Size = 3 }, // e2
            new Opcode(){ Extension = null, DisasmFormat = "ex (sp),hl",        Size = 1 }, // e3
            new Opcode(){ Extension = null, DisasmFormat = "call po,${0:x4}",   Size = 3 }, // e4
            new Opcode(){ Extension = null, DisasmFormat = "push hl",           Size = 1 }, // e5
            new Opcode(){ Extension = null, DisasmFormat = "and ${0:x2}",       Size = 2 }, // e6
            new Opcode(){ Extension = null, DisasmFormat = "rst $20",           Size = 1 }, // e7
            new Opcode(){ Extension = null, DisasmFormat = "ret pe",            Size = 1 }, // e8
            new Opcode(){ Extension = null, DisasmFormat = "jp (hl)",           Size = 1 }, // e9
            new Opcode(){ Extension = null, DisasmFormat = "jp pe,${0:x4}",     Size = 3 }, // ea
            new Opcode(){ Extension = null, DisasmFormat = "ex de,hl",          Size = 1 }, // eb
            new Opcode(){ Extension = null, DisasmFormat = "call pe,${0:x4}",   Size = 3 }, // ec
            new Opcode(){ Extension = null, DisasmFormat = null,                Size = 2 }, // ed
            new Opcode(){ Extension = null, DisasmFormat = "xor ${0:x2}",       Size = 2 }, // ee
            new Opcode(){ Extension = null, DisasmFormat = "rst $28",           Size = 1 }, // ef
            new Opcode(){ Extension = null, DisasmFormat = "ret p",             Size = 1 }, // f0
            new Opcode(){ Extension = null, DisasmFormat = "pop af",            Size = 1 }, // f1
            new Opcode(){ Extension = null, DisasmFormat = "jp p,${0:x4}",      Size = 3 }, // f2
            new Opcode(){ Extension = null, DisasmFormat = "di",                Size = 1 }, // f3
            new Opcode(){ Extension = null, DisasmFormat = "call p,${0:x4}",    Size = 3 }, // f4
            new Opcode(){ Extension = null, DisasmFormat = "push af",           Size = 1 }, // f5
            new Opcode(){ Extension = null, DisasmFormat = "or ${0:x2}",        Size = 2 }, // f6
            new Opcode(){ Extension = null, DisasmFormat = "rst $30",           Size = 1 }, // f7
            new Opcode(){ Extension = null, DisasmFormat = "ret m",             Size = 1 }, // f8
            new Opcode(){ Extension = null, DisasmFormat = "ld sp,hl",          Size = 1 }, // f9
            new Opcode(){ Extension = null, DisasmFormat = "jp m,${0:x4}",      Size = 3 }, // fa
            new Opcode(){ Extension = null, DisasmFormat = "ei",                Size = 1 }, // fb
            new Opcode(){ Extension = null, DisasmFormat = "call m,${0:x4}",    Size = 3 }, // fc
            new Opcode(){ Extension = null, DisasmFormat = null,                Size = 2 }, // fd
            new Opcode(){ Extension = null, DisasmFormat = "cp ${0:x2}",        Size = 2 }, // fe
            new Opcode(){ Extension = null, DisasmFormat = "rst $38",           Size = 1 }  // ff
        };

        private FormatBuilder[] _builders;

        #endregion
    }
}