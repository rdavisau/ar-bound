using System;
using System.Net.Http;
using ARKitMeetup.Models;
using CoreGraphics;
using Foundation;
using SpriteKit;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using System.Threading.Tasks;
using ARKitMeetup.Helpers;

namespace ARKitMeetup.Helpers
{
    public class ShaderScene : SKScene
    {
        private static Random _r = new Random();
    
        public static ShaderScene Random()
        {
            var bgs = NSBundle.MainBundle.PathsForResources(".png", "eb_bg");
            var bg = bgs[_r.Next(0, bgs.Length)];
            var img = UIImage.FromFile(bg);
            
            return new ShaderScene(img);
        }
    
        private Dictionary<string, float> _defaultParameters =
            new Dictionary<string, float>
            {
                ["u_l1_amp"] = .25f,
                ["u_l1_freq"] = 1.5f,
                ["u_l1_scale"] = 1f,

                ["u_l2_amp"] = .5f,
                ["u_l2_freq"] = .75f,
                ["u_l2_scale"] = 1f,

                ["u_blend"] = .15f
            };
            
        public ShaderScene(UIImage forImage, Dictionary<string,float> parameterOverrides = null)
        {
            var parameters = _defaultParameters.ToDictionary(x => x.Key, x => x.Value);            
            foreach (var po in parameterOverrides ?? new Dictionary<string, float>())
                parameters[po.Key] = po.Value;

            var tex = SKTexture.FromImage(forImage);
            var node = new SKSpriteNode
            {
                Texture = tex,
                Size = new CGSize(100, 100),
                AnchorPoint = CGPoint.Empty,
            };

            node.Shader = GetShaderForParameters(parameters);

            AddNodes(node);

            Size = new CGSize(100, 100); 
        }

        private SKShader GetShaderForParameters(Dictionary<string, float> parameters)
        {
            var uniforms =
               parameters
                .Select(kvp => SKUniform.Create(kvp.Key, kvp.Value))
                .ToArray();
                
            var shader = SKShader.FromShaderSourceCode
            (
               @"
                    void main() {
                    
                        // layer 1
                        vec2 coord = v_tex_coord; 
    
                        float time1 = u_time; 
                        bool isOtherLine = mod(floor(coord.y * 150.), 2.) == 0.;
                        float disty = u_l1_amp * sin(u_l1_freq * coord.x + u_l1_scale * time1); 
    
                        vec4 c1 = texture2D(u_texture, vec2(coord.x + disty, coord.y - time1 / 4.));
                        if (!isOtherLine)
                            c1 = texture2D(u_texture, vec2(coord.x - disty, coord.y - time1 / 4.)); 
                    
                        // layer 2
                        vec2 coord2 = v_tex_coord; 
                        
                        float time2 = u_time * 2.;
                        float y2 = coord.y - u_time / 2.; 
    
                        float disty2 = u_l2_amp * sin(u_l2_freq * y2 + u_l2_scale * time2); 
    
                        vec4 c2 = texture2D(u_texture, vec2(mod(coord2.x + disty2, 1.), mod(coord2.y + disty, 1.)));
                    
                        gl_FragColor = mix(c1, c2, u_blend); 
                     } 
                         
                ",
                uniforms );

            return shader;
        } 
    }
}
