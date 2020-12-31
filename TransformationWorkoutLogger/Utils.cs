using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using static Android.Views.ViewGroup;

namespace TransformationWorkoutLogger
{
    public class Utils
    {
        int numDifferentHeightSizes;
      //  int[] numEachSize;
        int[] bottomPaddingEachSizeMinusFooter;
        int[] topMarginAmounts;
        int[] heightEachSize;
        int widthEachGalleryImage;

        int widthMenu;
        int widthSelections;
        //float[] sideMarginEachSize;
        int height;
        int width;

        public Utils(int width, int height)
        {
            this.height = height;
            this.width = width;

            setWidthMenu(.3);
            setWidthSelections(.5);

        }

        public int getIntSizeFromPercentage(int valueMax, double percentage)
        {
            return int.Parse(Math.Floor((double)valueMax * percentage).ToString());


        }
   
        public void setHeightClasses(int numHeights)
        {
            numDifferentHeightSizes = numHeights;

            topMarginAmounts = new int[numHeights];
            bottomPaddingEachSizeMinusFooter = new int[numHeights - 1];
            heightEachSize = new int[numHeights];

        }

        public void setWidthMenu(double Percentage)
        {

            widthMenu = getIntSizeFromPercentage(width, .3);
        }

        public void setWidthSelections(double Percentage)
        {
            widthSelections = getIntSizeFromPercentage(width, .5);

        }
        public void setSingularWidth(double Percentage)
        {
            widthEachGalleryImage = getIntSizeFromPercentage(width, .333);

        }

        public void setMenuLLViewDimensions(View v)
        {

            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;
            // mlp.TopMargin = getTopMargin(heightIndex);
            mlp.Height = v.Height;
            mlp.Width = widthMenu;
            mlp.LeftMargin = 10;
            mlp.RightMargin = 10;
            v.LayoutParameters = mlp;

        }
        public void setSelectionLLViewDimensions(View v)
        {

            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;
            // mlp.TopMargin = getTopMargin(heightIndex);
            mlp.Height = v.Height;
            mlp.Width = widthSelections;
            mlp.LeftMargin = 10;
            mlp.RightMargin = 10;
            v.LayoutParameters = mlp;

        }
        public void setGalleryItemViewDimensions(View v)
        {
            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;
            // mlp.TopMargin = getTopMargin(heightIndex);
            mlp.Height = widthEachGalleryImage - 40;
            mlp.Width = widthEachGalleryImage - 40;
            mlp.LeftMargin = 10;
            mlp.RightMargin = 10;
            v.LayoutParameters = mlp;
            



        }

   /*     public void setVideoThumbnailTextLocation(VideoThumbnailedView v)
        {
            int[] location = new int[2];
            int[] location2 = new int[2];
            v.GetLocationOnScreen(location);
            v.GetLocationInWindow(location2);


            int width = v.LayoutParameters.Width;

            v.setTextLocation(location[0] + 300, location[1] + 300);
          //  v.setTextLocation(int.Parse((((location2[0] + v.Width)) - v.textWidth -30).ToString()), location2[1]  - int.Parse(v.textHeight.ToString()) -10);
            v.setTextLocation(int.Parse((v.Width- v.textWidth -20).ToString()), (v.Height) - 50 - int.Parse(Math.Ceiling(v.textHeight).ToString()));




        }
        public void setThumbnailTextLocation(ThumbnailedView v)
        {
            int[] location = new int[2];
            int[] location2 = new int[2];
            v.GetLocationOnScreen(location);
            v.GetLocationInWindow(location2);


            int width = v.LayoutParameters.Width;

            v.setTextLocation(location[0] + 300, location[1] + 300);
            //   v.setTextLocation(int.Parse((Math.Ceiling(((location2[0] + v.Width) / 2) - v.textWidth / 2).ToString())), location2[1]);

            v.setTextLocation(int.Parse((Math.Ceiling(((location2[0] + v.Width) / 2) - v.textWidth / 2).ToString())), v.Height/2);



        }
        public void setViewTextLocation(ThumbnailedView v)
        {
          //  int[] location = new int[2];
            int[] location2 = new int[2];
          //  v.GetLocationOnScreen(location);
            v.GetLocationInWindow(location2);


            int width = v.LayoutParameters.Width;

            // v.setTextLocation(location[0] +300, location[1] +300);
            v.setTextLocation(int.Parse((Math.Ceiling(((location2[0] + v.Width) / 2) - v.textWidth / 2).ToString())), 0);




        }
        public void setThumbnailImageViewDimensionsHeight(ThumbnailedView v, int heightIndex)
        {

            v.LayoutParameters.Height = getHeightSize(heightIndex);
            v.setWidthAndHeight(v.LayoutParameters.Height, v.LayoutParameters.Height);

        }
   */
        public void setHeight(int heightIndex, double percentage)
        {
            heightEachSize[heightIndex] = getIntSizeFromPercentage(height, percentage);


        }
        public void setTopMargin(int marginIndex, double percentage)
        {
            topMarginAmounts[marginIndex] = getIntSizeFromPercentage(height, percentage);
        }

        public void setImageViewDimensionsHeight(View v, int heightIndex)
        {

            v.LayoutParameters.Height = getHeightSize(heightIndex);

        }
        public void setGoneViewMarginsToZero(View v)
        {

            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;

            mlp.SetMargins(0, 0, 0, 0);

        }
        public void setViewDimensionsHeight(View v, int heightIndex)
        {


            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;
          // mlp.TopMargin = getTopMargin(heightIndex);
            mlp.Height = getHeightSize(heightIndex);
           
            v.LayoutParameters = mlp;

        }

       
        public void setViewTopMargins(View v, int heightIndex)
        {
            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;
            mlp.TopMargin = getTopMargin(heightIndex);
            v.LayoutParameters = mlp;

        }
        public void setViewDimensions(View v, int heightIndex)
        {
          //  v.LayoutParameters.Height = getHeightSize(heightIndex);
         //   v.LayoutParameters.Width = getHeightSize(heightIndex);
            // MarginLa v.LayoutParameters as ViewGroup.MarginLayoutParams;

            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;
            mlp.TopMargin = getTopMargin(heightIndex);
            mlp.Height = getHeightSize(heightIndex)- mlp.TopMargin ;
            mlp.Width = getHeightSize(heightIndex) - mlp.TopMargin ;

            mlp.LeftMargin = 10;
            mlp.RightMargin = 10;
            v.LayoutParameters = mlp;
        }
        public void setTextViewDimensions(View v, int heightIndex)
        {
            //  v.LayoutParameters.Height = getHeightSize(heightIndex);
            //   v.LayoutParameters.Width = getHeightSize(heightIndex);
            // MarginLa v.LayoutParameters as ViewGroup.MarginLayoutParams;

            MarginLayoutParams mlp = (MarginLayoutParams)v.LayoutParameters;
            mlp.TopMargin = getTopMargin(heightIndex);
            mlp.Height = getHeightSize(heightIndex) - mlp.TopMargin;
            //  mlp.Width =int.Parse(((TextView)v).TextSize.ToString()) * ((TextView)v).Text.Length;
            mlp.Width = 200;
            //mlp.Width = getHeightSize(heightIndex) - mlp.TopMargin;
            v.LayoutParameters = mlp;
        }



        public int getHeightSize(int heightIndex)
        {

            return heightEachSize[heightIndex];
        }
        public int getTopMargin(int heightIndex)
        {

            return topMarginAmounts[heightIndex];

        }
    }
}