using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class ExampleCodeLoader: MonoBehaviour
    {
        private const int WRAPPER_OBJECT_CODE_ID = 1;
        private const int PROXY_CODE_ID = 2;

        private const string WRAPPER_OBJECT_CODE =
@"package com.madware.local_video.wrappers

import androidx.annotation.Keep
import com.madware.external_texture.wrappers.AbstractExternalTexturesPoolWrapperObject
import com.madware.external_texture.wrappers.ITextureIdListenerWrapperProxy

abstract class AbstractVlcPlayerWrapperObject(
        val pool: AbstractExternalTexturesPoolWrapperObject,
        val proxy: ITextureIdListenerWrapperProxy) {
    @Keep
    abstract fun play()
    @Keep
    abstract fun pause()
    @Keep
    abstract fun playing(): Boolean

    @Keep
    abstract fun duration(): Long
    @Keep
    abstract fun position(): Long
    @Keep
    abstract fun setPosition(position: Long)

    @Keep
    abstract fun playCurrentFile(filePath: String, isHls: Boolean)

    @Keep
    abstract fun release()

}
";
        
        private const string PROXY_CODE = 
@"package com.madware.local_video.wrappers

interface IVlcPlayerListener {
    fun onVideoEnded()
}";
        
        [SerializeField] private TMP_InputField _inputField;

        public void LoadExampleCode(int index)
        {
            _inputField.text = index switch
            {
                WRAPPER_OBJECT_CODE_ID => WRAPPER_OBJECT_CODE,
                PROXY_CODE_ID => PROXY_CODE,
                _ => ""
            };
        }

    }
}