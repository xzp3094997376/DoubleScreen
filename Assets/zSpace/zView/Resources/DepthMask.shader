//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

Shader "zSpace/zView/DepthMask"
{
    SubShader
    {
        // Only draw to the depth buffer.
        ColorMask 0
        ZWrite On

        Pass
        {
            // Do nothing in this pass.
        }
    }
}