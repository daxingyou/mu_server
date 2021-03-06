﻿using System;

namespace DotNetDetour
{
    
    public class LDasm
    {
        
        private static byte cflags(byte op)
        {
            return LDasm.flags_table[(int)op];
        }

        
        private static byte cflags_ex(byte op)
        {
            return LDasm.flags_table_ex[(int)op];
        }

        
        public unsafe static uint SizeofMin5Byte(void* code)
        {
            uint Result = 0U;
            LDasm.ldasm_data data = default(LDasm.ldasm_data);
            bool is64 = IntPtr.Size == 8;
            uint Length;
            do
            {
                Length = LDasm.ldasm(code, data, is64);
                byte* pOpcode = (byte*)code + data.opcd_offset;
                Result += Length;
                if (Result >= 5U)
                {
                    break;
                }
                if (Length == 1U && *pOpcode == 204)
                {
                    break;
                }
                code = (void*)((byte*)code + (ulong)Length);
            }
            while (Length > 0U);
            return Result;
        }

        
        private unsafe static uint ldasm(void* code, LDasm.ldasm_data ld, bool is64)
        {
            byte* p = (byte*)code;
            byte pr_67;
            byte pr_66;
            byte s;
            byte rexw = s = (pr_66 = (pr_67 = 0));
            uint result;
            if ((int)code == 0)
            {
                result = 0U;
            }
            else
            {
                while ((LDasm.cflags(*p) & 128) != 0)
                {
                    if (*p == 102)
                    {
                        pr_66 = 1;
                    }
                    if (*p == 103)
                    {
                        pr_67 = 1;
                    }
                    p++;
                    s += 1;
                    ld.flags |= 2;
                    if (s == 15)
                    {
                        ld.flags |= 1;
                        return (uint)s;
                    }
                }
                if (is64 && *p >> 4 == 4)
                {
                    ld.rex = *p;
                    rexw = (byte)(ld.rex >> 3 & 1);
                    ld.flags |= 4;
                    p++;
                    s += 1;
                }
                if (is64 && *p >> 4 == 4)
                {
                    ld.flags |= 1;
                    s += 1;
                    result = (uint)s;
                }
                else
                {
                    ld.opcd_offset = (byte)((long)((byte*)p - (byte*)code));
                    ld.opcd_size = 1;
                    byte op = *(p++);
                    s += 1;
                    byte f;
                    if (op == 15)
                    {
                        op = *(p++);
                        s += 1;
                        ld.opcd_size += 1;
                        f = LDasm.cflags_ex(op);
                        if ((f & 128) != 0)
                        {
                            ld.flags |= 1;
                            return (uint)s;
                        }
                        if ((f & 16) != 0)
                        {
                            op = *(p++);
                            s += 1;
                            ld.opcd_size += 1;
                        }
                    }
                    else
                    {
                        f = LDasm.cflags(op);
                        if (op >= 160 && op <= 163)
                        {
                            pr_66 = pr_67;
                        }
                    }
                    if ((f & 64) != 0)
                    {
                        byte mod = (byte)(*p >> 6);
                        byte ro = (byte)((*p & 56) >> 3);
                        byte rm = (byte)(*p & 7);
                        ld.modrm = *(p++);
                        s += 1;
                        ld.flags |= 8;
                        if (op == 246 && (ro == 0 || ro == 1))
                        {
                            f |= 1;
                        }
                        if (op == 247 && (ro == 0 || ro == 1))
                        {
                            f |= 8;
                        }
                        if (mod != 3 && rm == 4 && (is64 || pr_67 == 0))
                        {
                            ld.sib = *(p++);
                            s += 1;
                            ld.flags |= 16;
                            if ((ld.sib & 7) == 5 && mod == 0)
                            {
                                ld.disp_size = 4;
                            }
                        }
                        switch (mod)
                        {
                            case 0:
                                if (is64)
                                {
                                    if (rm == 5)
                                    {
                                        ld.disp_size = 4;
                                        if (is64)
                                        {
                                            ld.flags |= 128;
                                        }
                                    }
                                }
                                else if (pr_67 != 0)
                                {
                                    if (rm == 6)
                                    {
                                        ld.disp_size = 2;
                                    }
                                }
                                else if (rm == 5)
                                {
                                    ld.disp_size = 4;
                                }
                                break;
                            case 1:
                                ld.disp_size = 1;
                                break;
                            case 2:
                                if (is64)
                                {
                                    ld.disp_size = 4;
                                }
                                else if (pr_67 != 0)
                                {
                                    ld.disp_size = 2;
                                }
                                else
                                {
                                    ld.disp_size = 4;
                                }
                                break;
                        }
                        if (ld.disp_size > 0)
                        {
                            ld.disp_offset = (byte)((long)((byte*)p - (byte*)code));
                            p += ld.disp_size;
                            s += ld.disp_size;
                            ld.flags |= 32;
                        }
                    }
                    if (rexw != 0 && (f & 8) != 0)
                    {
                        ld.imm_size = 8;
                    }
                    else if ((f & 4) != 0 || (f & 8) != 0)
                    {
                        ld.imm_size = (byte)(4 - ((int)pr_66 << 1));
                    }
                    ld.imm_size += (byte)(f & 3);
                    if (ld.imm_size != 0)
                    {
                        s += ld.imm_size;
                        ld.imm_offset = (byte)((long)((byte*)p - (byte*)code));
                        ld.flags |= 64;
                        if ((f & 32) != 0)
                        {
                            ld.flags |= 128;
                        }
                    }
                    if (s > 15)
                    {
                        ld.flags |= 1;
                    }
                    result = (uint)s;
                }
            }
            return result;
        }

        
        private const int F_INVALID = 1;

        
        private const int F_PREFIX = 2;

        
        private const int F_REX = 4;

        
        private const int F_MODRM = 8;

        
        private const int F_SIB = 16;

        
        private const int F_DISP = 32;

        
        private const int F_IMM = 64;

        
        private const int F_RELATIVE = 128;

        
        private const int OP_NONE = 0;

        
        private const int OP_INVALID = 128;

        
        private const int OP_DATA_I8 = 1;

        
        private const int OP_DATA_I16 = 2;

        
        private const int OP_DATA_I16_I32 = 4;

        
        private const int OP_DATA_I16_I32_I64 = 8;

        
        private const int OP_EXTENDED = 16;

        
        private const int OP_RELATIVE = 32;

        
        private const int OP_MODRM = 64;

        
        private const int OP_PREFIX = 128;

        
        private static byte[] flags_table = new byte[]
        {
            64,
            64,
            64,
            64,
            1,
            4,
            0,
            0,
            64,
            64,
            64,
            64,
            1,
            4,
            0,
            0,
            64,
            64,
            64,
            64,
            1,
            4,
            0,
            0,
            64,
            64,
            64,
            64,
            1,
            4,
            0,
            0,
            64,
            64,
            64,
            64,
            1,
            4,
            128,
            0,
            64,
            64,
            64,
            64,
            1,
            4,
            128,
            0,
            64,
            64,
            64,
            64,
            1,
            4,
            128,
            0,
            64,
            64,
            64,
            64,
            1,
            4,
            128,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            64,
            64,
            128,
            128,
            128,
            128,
            4,
            68,
            1,
            65,
            0,
            0,
            0,
            0,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            33,
            65,
            68,
            65,
            65,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            6,
            0,
            0,
            0,
            0,
            0,
            1,
            8,
            1,
            8,
            0,
            0,
            0,
            0,
            1,
            4,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            65,
            65,
            2,
            0,
            64,
            64,
            65,
            68,
            3,
            0,
            2,
            0,
            0,
            1,
            0,
            0,
            64,
            64,
            64,
            64,
            1,
            1,
            0,
            0,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            33,
            33,
            33,
            33,
            1,
            1,
            1,
            1,
            36,
            36,
            6,
            33,
            0,
            0,
            0,
            0,
            128,
            0,
            128,
            128,
            0,
            0,
            64,
            64,
            0,
            0,
            0,
            0,
            0,
            0,
            64,
            64
        };

        
        private static byte[] flags_table_ex = new byte[]
        {
            64,
            64,
            64,
            64,
            128,
            0,
            0,
            0,
            0,
            0,
            128,
            0,
            128,
            64,
            128,
            65,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            128,
            128,
            128,
            128,
            128,
            128,
            0,
            64,
            64,
            64,
            64,
            80,
            128,
            64,
            128,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            0,
            0,
            0,
            0,
            0,
            0,
            128,
            0,
            80,
            128,
            81,
            128,
            128,
            128,
            128,
            128,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            65,
            65,
            65,
            65,
            64,
            64,
            64,
            0,
            64,
            64,
            128,
            128,
            64,
            64,
            64,
            64,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            36,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            0,
            0,
            0,
            64,
            65,
            64,
            128,
            128,
            0,
            0,
            0,
            64,
            65,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            65,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            65,
            64,
            65,
            65,
            65,
            64,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            64,
            128
        };

        
        private struct ldasm_data
        {
            
            public byte flags;

            
            public byte rex;

            
            public byte modrm;

            
            public byte sib;

            
            public byte opcd_offset;

            
            public byte opcd_size;

            
            public byte disp_offset;

            
            public byte disp_size;

            
            public byte imm_offset;

            
            public byte imm_size;
        }
    }
}
