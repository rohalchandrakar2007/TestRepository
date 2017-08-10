//
//  main_process.cpp
//  SFM_Test_01
//
//  Created by Rohal Chandrakar on 7/25/17.
//  Copyright © 2017 Rohal Chandrakar. All rights reserved.
//
#include "opencv2/objdetect/objdetect.hpp"
#include <opencv2/calib3d.hpp>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>


#include <iostream>
#include <fstream>
#include <string>
#include <map>

using namespace std;
using namespace cv;

int const RESIZED_IMAGE_WIDTH = 1000;
int const BLOCK_SIZE = RESIZED_IMAGE_WIDTH * ((float)3 / (float)100);
int const EDGE_DETECTOR_MIN_THRESHOLD = 40;
int const EDGE_DETECTOR_MAX_THRESHOLD = 40;
String face_cascade_name = "/home/ubuntu/opencv/data/haarcascade_frontalface_alt.xml";
CascadeClassifier face_cascade;


void saveLowQualityImage(Mat x, string path, int quality){
    vector<int> compression_params;
    compression_params.push_back(CV_IMWRITE_JPEG_QUALITY);
    compression_params.push_back(quality);
    cv::imwrite(path, x, compression_params);
}


/** @function detectAndDisplay */
vector<Rect> detectAndDisplay( Mat frame )
{
    std::vector<Rect> faces;
    Mat frame_gray;
    
    cvtColor( frame, frame_gray, CV_BGR2GRAY );
    equalizeHist( frame_gray, frame_gray );
    
    //-- Detect faces
    face_cascade.detectMultiScale( frame_gray, faces, 1.1, 2, 0|CV_HAAR_SCALE_IMAGE, Size(30, 30) );
    
    /*for( size_t i = 0; i < faces.size(); i++ )
    {
        Point center( faces[i].x + faces[i].width*0.5, faces[i].y + faces[i].height*0.5 );
        ellipse( frame, center, Size( faces[i].width*0.5, faces[i].height*0.5), 0, 0, 360, Scalar( 255, 0, 255 ), 4, 8, 0 );
        
        Mat faceROI = frame_gray( faces[i] );
        std::vector<Rect> eyes;
    }*/
    //-- Show what you got
    //imshow( window_name, frame );
    
    return faces;
}


int main(int argc, char* argv[])
{
    /*if(argc < 1){
        cout << "arvind please enter the file path as parameter" << endl;
        return 0;
    }
    */
    
    //Mat x = imread(argv[1]);
    
    //-- 1. Load the cascades
    if( !face_cascade.load( face_cascade_name ) ){ printf("--(!)Error loading\n"); return -1; };
    
    Mat x = imread("/home/ubuntu/opencv/data/living_small.jpg");
    Mat x_copy = x.clone();
    int new_height = RESIZED_IMAGE_WIDTH * ((float)x_copy.rows / (float)x_copy.cols);
    resize(x_copy, x_copy, Size(RESIZED_IMAGE_WIDTH, new_height));
    vector<Rect> faces = detectAndDisplay(x_copy);
    
    cvtColor(x_copy, x_copy, COLOR_BGR2GRAY);
    Mat x_edges;
    Canny(x_copy, x_edges, EDGE_DETECTOR_MIN_THRESHOLD, EDGE_DETECTOR_MAX_THRESHOLD, 3);
    
    
    Mat invSrc =  cv::Scalar::all(255) - x_edges;
    //saveLowQualityImage(invSrc, "/Users/Rohal/Desktop/saved.jpg", 20);
    

    
    for(int i = BLOCK_SIZE; i < x_copy.rows - BLOCK_SIZE; i = i + BLOCK_SIZE){
        for(int j = 0; j < x_copy.cols - BLOCK_SIZE; j = j + BLOCK_SIZE){
            
            bool paintBlock = 1;
            
            for(int x = i; x < i + BLOCK_SIZE; x++){
                for(int y = j; y < j + BLOCK_SIZE; y++){
                    Vec3b color = x_edges.at<Vec3b>(x,y);
                    if(color[0] > 100 || color[1] > 100 || color[2] > 100){
                        paintBlock = 0;
                        break;
                    }
                }
                if(paintBlock == 0)
                    break;
            }
            if(paintBlock == 1){
                for(int x = i; x < i + BLOCK_SIZE; x++)
                    for(int y = j; y < j + BLOCK_SIZE; y++){
                        x_edges.at<Vec3b>(x,y)[0] = 255;
                        x_edges.at<Vec3b>(x,y)[1] = 255;
                        x_edges.at<Vec3b>(x,y)[2] = 255;
                    }
            }
            
        }
    }
    
    
    int maxLen = -INFINITY;
    cvtColor(x_edges, x_edges, COLOR_GRAY2RGB);
    pair<int, int> optimal;
    //map<int, pair<int, int>> res;
    vector<int> res;
    for(int i = new_height / 4; i < new_height * 6 / 10; i++){
        for(int j = 0; j < x_edges.cols; j++){
            
            Vec3b color = x_edges.at<Vec3b>(i,j);
            if(color[0] == 255 && color[1] == 255 && color[2] == 255){
                int max_pix = 0;
                for(int k = 0; k <= new_height / 4; k++){
                    
                    int f_x = i + k;
                    int f_y = j + k;
                    if(f_y < x_edges.cols - 1){
                        Vec3b color_inner = x_edges.at<Vec3b>(f_x,f_y);
                        
                        if(color_inner[0] == 255 && color_inner[1] == 255 && color_inner[2] == 255)
                            max_pix++;
                        else
                            break;
                    }else
                        break;
                }
                
                if(max_pix > BLOCK_SIZE){
                    for(int x = i; x < i + max_pix; x++)
                        for(int y = j; y < j + max_pix; y++){
                            x_edges.at<Vec3b>(x,y)[0] = 200;
                            x_edges.at<Vec3b>(x,y)[1] = 200;
                            x_edges.at<Vec3b>(x,y)[2] = 200;
                        }
                    
                    
                    maxLen = max_pix;
                    res.push_back(max_pix);
                    res.push_back(i);
                    res.push_back(j);
                }
                
            }
            
            
        }
    }
    int w_fact = (float)x.cols / (float)RESIZED_IMAGE_WIDTH;
    int h_fact = (float)x.rows / (float)new_height;
    for(int xi = 0 ; xi < res.size(); xi += 3){
        rectangle( x_edges,Point( res[xi + 2], res[xi + 1] ),Point( res[xi + 2] + res[xi], res[xi + 1] + res[xi]),Scalar( 0, 0, 255 ),-1,8);
        
        cout << res[xi] << " " << res[xi + 2] * w_fact << " " << res[xi + 1] * h_fact << " ";
    }
    cout << "faces: ";
    for(int f = 0; f < faces.size(); f++){
        cout << (faces[f].x + faces[f].width * 0.5) * w_fact << " " << (faces[f].y + faces[f].height * 0.5) * h_fact << " ";
    }
    
    imshow("m_w", x_edges);
    
    waitKey(0);
    
    return 0;
}
