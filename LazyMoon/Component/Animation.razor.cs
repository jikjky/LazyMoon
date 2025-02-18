﻿using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace LazyMoon.Component
{
    partial class Animation
    {
        public enum EAnimations
        {
            FadeIn,
            FadeOut,
            FadeInLeft,
            FadeInDown,
            SlideLeft
        }
        public enum EFillMode
        {
            /// <summary>
            /// 적용 안함
            /// </summary>
            None,

            /// <summary>
            /// 100% 도달후 마지막 키프레임 유지
            /// </summary>
            Forwards,

            /// <summary>
            /// 애니메이션 스타일을 시작전에 미리 적용
            /// </summary>
            Backwards,

            /// <summary>
            /// 둘다 적용
            /// </summary>
            Both
        }

        [Parameter]
#nullable enable
        public RenderFragment? ChildContent { get; set; }
#nullable disable

        /// <summary>
        /// 애니메이션 종류
        /// </summary>
        [Parameter]
        public EAnimations AnimationType { get; set; } = EAnimations.FadeIn;

        /// <summary>
        /// Fill Mode
        /// </summary>
        [Parameter]
        public EFillMode FillMode { get; set; } = EFillMode.None;

        /// <summary>
        /// 처음 시작 여부를 Manual로 할지 자동으로 시작할지
        /// </summary>
        [Parameter]
        public bool IsManual { get; set; }

        /// <summary>
        /// 에니메이션 시작전 표시여부
        /// </summary>
        [Parameter]
        public bool IsVisible { get; set; } = false;

        /// <summary>
        /// 시작 딜레이 시간 (ms)
        /// </summary>
        [Parameter]
        public int Delay { get; set; } = 0;

        /// <summary>
        /// 애니메이션 시간 (ms)
        /// </summary>
        [Parameter]
        public int Duration { get; set; } = 1000;

        [Parameter]
        public bool Done { get; set; } = false;

        private bool mFirst = false;
        private bool DoAnimation { get; set; } = false;



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (IsManual == false)
                    await Run();
                StateHasChanged();
            }
        }

        public async Task Run()
        {
            StateHasChanged();
            await Task.Delay(Delay);
            mFirst = true;
            DoAnimation = true;
            StateHasChanged();
            await Task.Delay(Duration);
            if (FillMode != EFillMode.Forwards)
            {
                DoAnimation = false;
                Done = true;
            }
            StateHasChanged();
        }
    }
}
