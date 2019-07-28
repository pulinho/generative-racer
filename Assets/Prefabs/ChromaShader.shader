Shader "Unlit/ChromaShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _FirstColor ("Color in First", Color) = (1, 0, 0, 1)
        _SecondColor("Color in Second", Color) = (1, 1, 0, 1)
        _ThirdColor ("Color in Third", Color) = (0, 1, 0, 1)
        _FourthColor ("Color in Fourth", Color) = (0, 1, 1, 1)
        _FifthColor("Color in Fifth", Color) = (0, 0, 1, 1)
        _SixthColor ("Color in Sixth", Color) = (1, 0, 1, 1)
          
          
        _FirstDistance ("First Distance", Float) = 1
        _SecondDistance ("Second Distance", Float) = 2
        _ThirdDistance ("Third Distance", Float) = 4
        _FourthDistance ("Fourth Distance", Float) = 8
        _FifthDistance ("Fifth Distance", Float) = 16
        _SixthDistance ("Sixth Distance", Float) = 32
          
        _Threshold ("Threshold", Range(0.0, 1.0)) = 1.
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
 
          
        CGPROGRAM
        #pragma surface surf Lambert
  
        sampler2D _MainTex;
  
        struct Input 
        {
            float2 uv_MainTex;
            float3 worldPos;
        };
  
        float _FirstDistance;
        float _SecondDistance;
        float _ThirdDistance;
        float _FourthDistance;
        float _FifthDistance;
        float _SixthDistance;
        half4 _FirstColor;
        half4 _SecondColor;
        half4 _ThirdColor;
        half4 _FourthColor;
        half4 _FifthColor;
        half4 _SixthColor;
        half4 _distanceColor;
        float _Threshold;
         
        void surf (Input IN, inout SurfaceOutput o) 
        {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
                          
            if(dist < _FirstDistance){
                //half weight = 1.0;
                _distanceColor = _FirstColor;
            }
             
            if(dist >= _FirstDistance && dist < _SecondDistance){
                half weight = (dist - _FirstDistance) / (_SecondDistance - _FirstDistance);
                _distanceColor = lerp(_FirstColor, _SecondColor, weight);
            }
                 
            if(dist >= _SecondDistance && dist < _ThirdDistance){
                half weight = (dist - _SecondDistance) / (_ThirdDistance - _SecondDistance);
                _distanceColor = lerp(_SecondColor, _ThirdColor, weight);
            }
                 
            if(dist >= _ThirdDistance && dist < _FourthDistance){
                half weight = (dist - _ThirdDistance) / (_FourthDistance - _ThirdDistance);
                _distanceColor = lerp(_ThirdColor, _FourthColor, weight);
            }
             
            if(dist >= _FourthDistance && dist < _FifthDistance){
                half weight = (dist - _FourthDistance) / (_FifthDistance - _FourthDistance);
                _distanceColor = lerp(_FourthColor, _FifthColor, weight);
            }
             
            if(dist >= _FifthDistance && dist < _SixthDistance){
                half weight = (dist - _FifthDistance) / (_SixthDistance - _FifthDistance);
                _distanceColor = lerp(_FifthColor, _SixthColor, weight);
            }
             
            if(dist >= _SixthDistance){
                //half weight = 1;
                _distanceColor = _SixthColor;
            }
             
            o.Albedo = c.rgb * _distanceColor.rgb;
            o.Alpha = c.a;
            o.Emission = c.rgb * pow(_distanceColor.rgb, _Threshold);
        }
         
        ENDCG
    } 
    FallBack "Diffuse"
}
