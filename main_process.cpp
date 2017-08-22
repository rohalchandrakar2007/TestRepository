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
int const BLOCK_SIZE = RESIZED_IMAGE_WIDTH * ((float)2 / (float)100);
int const EDGE_DETECTOR_MIN_THRESHOLD = 40;
int const EDGE_DETECTOR_MAX_THRESHOLD = 40;
string outputFileDir = "/Users/Rohal/Desktop/";
string inputFilePath = "/Users/Rohal/Desktop/input.jpg";
string inputBaseFile = "/Users/Rohal/Downloads/base.jpg";
String face_cascade_name = "/Users/Rohal/Desktop/haarcascade_frontalface_alt.xml";
string outputFileName = "";
String output_config = "/Users/Rohal/Desktop/output_config.neo";
CascadeClassifier face_cascade;
string msg = "", status = "0", data = "", spaces = "", faces_str = "";
int processingtime = 0;

void initArgementValues(string __path){
    
    
}


vector<string> split(const string &text, char sep) {
    vector<string> tokens;
    std::size_t start = 0, end = 0;
    while ((end = text.find(sep, start)) != string::npos) {
        tokens.push_back(text.substr(start, end - start));
        start = end + 1;
    }
    tokens.push_back(text.substr(start));
    return tokens;
}


void saveLowQualityImage(Mat x, string path, int quality){
    vector<int> compression_params;
    compression_params.push_back(CV_IMWRITE_JPEG_QUALITY);
    compression_params.push_back(quality);
    cv::imwrite(path, x, compression_params);
}


/* resize image and save it */
void resizeAndSave(Mat __x, int width, int height, string __path, int __quality){
    Mat temp;
    resize(__x, temp, Size(width, height));
    saveLowQualityImage(temp, __path, __quality);
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
    
    return faces;
}

void saveDesiredImages(Mat __x, string __pathSave, Mat __baseMat, string __fileName){
    Mat baseMat = __baseMat;
    Mat x;
    int new_height = 4096 * ((float)__x.rows / (float)__x.cols);
    int new_width = 2048 * ((float)__x.cols / (float)__x.rows);
    //cout << __x.cols << "  " << __x.rows << endl;
    if(__x.cols < 2048 && __x.rows < 2048){
        __x.copyTo(baseMat(Rect((4096 / 2) - (__x.cols / 2), (2048 / 2) - (__x.rows / 2), __x.cols, __x.rows)));
    }
    else{
        if(__x.cols > 2 * __x.rows){
            resize(__x, __x, Size(4096, new_height));
            //cout << "0 " << ((2048 / 2) - (__x.rows / 2)) << " " << __x.cols << " " << __x.rows << endl;
            __x.copyTo(baseMat(Rect(0, (2048 / 2) - (__x.rows / 2), __x.cols, __x.rows)));
        }else{
            resize(__x, __x, Size(new_width, 2048));
            //cout << "0 " << ((2048 / 2) - (__x.rows / 2)) << " " << __x.cols << " " << __x.rows << endl;
            __x.copyTo(baseMat(Rect((4096 / 2) - (__x.cols / 2), 0, __x.cols, __x.rows)));
        }
    }
    
    
    
    ifstream infile(output_config);
    string line;
    while (getline(infile, line))
    {
        std::istringstream iss(line);
        vector<string> strs1 = split(line, ':');
        string temp_name = strs1[0];
        vector<string> strs2 = split(strs1[1], 'x');
        int temp_width = stoi(strs2[0]);
        int temp_height = stoi(strs2[1]);
        
        if(temp_name == "low"){
            Mat x_edges;
            Canny(baseMat, x_edges, EDGE_DETECTOR_MIN_THRESHOLD, EDGE_DETECTOR_MAX_THRESHOLD, 3);
            Mat invSrc =  cv::Scalar::all(255) - x_edges;
            resizeAndSave(invSrc, temp_width, temp_height, __pathSave + __fileName + "-" + temp_name + "-" + strs2[0] + "-" + strs2[1] + ".jpg", 60);
        }
        else{
            resizeAndSave(baseMat, temp_width, temp_height, __pathSave + __fileName + "-" + temp_name + "-" + strs2[0] + "-" + strs2[1] + ".jpg", 60);
        }
    }
}


bool isPanorama(Mat __x){
    Mat x_edges;
    resize(__x, __x, Size(1024, 2048));
    Canny(__x, x_edges, 150, 150, 3);
    
    imshow("edges", x_edges);
    return 0;
}


void writeData(){
    string str = "";
    str = "{'status': " + status + ",'message': '" + msg + "','data': " + data + ", 'processingtime': " + to_string(processingtime) + "}";
    cout << str << endl;
}

int main(int argc, char* argv[])
{
    if(argc < 1){
        cout << "arvind please enter the file path as parameter" << endl;
        return 0;
    }
    
    
    inputFilePath = argv[1];
    outputFileDir = argv[2];
    outputFileName = argv[3];
    inputBaseFile = argv[4];
    face_cascade_name = argv[5];
    output_config = argv[6];
    
    
    
    
    clock_t start, end;
    // Load the cascades
    if( !face_cascade.load( face_cascade_name ) )
    {
        msg = "Error loading Haar file please chaeck the path...";
        writeData();
        return -1;
    };
    
    start = clock();
    
    //cout << "start clock" << endl;
    
    // load input image
    
    Mat x = imread(inputFilePath);
    if(! x.data )                              // Check for invalid input
    {
        msg = "Could not open or find the image...";
        writeData();
        return -1;
    }
    
    
    // load input base image
    
    Mat xBsaeImg = imread(inputBaseFile);
    if(! xBsaeImg.data )                              // Check for invalid input
    {
        msg = "Could not open or find the base image...";
        writeData();
        return -1;
    }
    
    saveDesiredImages(x, outputFileDir, xBsaeImg, outputFileName);
    
    
    //Mat x = imread(inputFilePath);
    //saveDesiredImages(x, outputFileDir, inputBaseFile, outputFileName);
    
    
    
    Mat x_copy = x.clone();
    int new_height = RESIZED_IMAGE_WIDTH * ((float)x_copy.rows / (float)x_copy.cols);
    resize(x_copy, x_copy, Size(RESIZED_IMAGE_WIDTH, new_height));
    vector<Rect> faces = detectAndDisplay(x_copy);
    
    cvtColor(x_copy, x_copy, COLOR_BGR2GRAY);
    Mat x_edges;
    Canny(x_copy, x_edges, EDGE_DETECTOR_MIN_THRESHOLD, EDGE_DETECTOR_MAX_THRESHOLD, 3);
    
    
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
    //cout << "space: ";
    spaces += "'spaces': [";
    for(int xi = 0 ; xi < res.size(); xi += 3){
        rectangle( x_edges,Point( res[xi + 2], res[xi + 1] ),Point( res[xi + 2] + res[xi], res[xi + 1] + res[xi]),Scalar( 0, 0, 255 ),-1,8);
        if(xi != 0)
            spaces += ",";
        spaces += "{'radius': " + to_string(res[xi]) + ", 'xcord': " + to_string((res[xi + 2] * w_fact)) + ", 'ycord': " + to_string(res[xi + 1] * h_fact) + "}";
        //cout << res[xi] << " " << res[xi + 2] * w_fact << " " << res[xi + 1] * h_fact << " ";
    }
    spaces += "]";
    //cout << spaces << endl;
    //imshow("m_w", x_edges);
    //imshow("m_wr", res);
    
    //cout << endl;
    //cout << "faces: ";
    faces_str += "'faces': [";
    for(int f = 0; f < faces.size(); f++){
        //cout << (faces[f].x + faces[f].width * 0.5) * w_fact << " " << (faces[f].y + faces[f].height * 0.5) * h_fact << " ";
        
        if(f != 0)
            faces_str += ",";
        faces_str += "{'xcord': " + to_string(((faces[f].x + faces[f].width * 0.5) * w_fact)) + ", 'ycord': " + to_string((faces[f].y + faces[f].height * 0.5) * h_fact) + "}";
    }
    faces_str += "]";
    //cout << faces_str;
    //cout << endl;
    
    data = "{"+ spaces + ", " + faces_str +"}";
    
    end = clock();
    double msecs = ((double) (end - start)) * 1000 / CLOCKS_PER_SEC;
    
    status = "1";
    msg = "completed";
    processingtime = msecs;
    //cout << "time taken : " << msecs << endl;
    
    writeData();
    //imshow("m_w", x_edges);
    
    //waitKey(0);
    
    return 0;
}
