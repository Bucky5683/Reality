#!/usr/bin/env python
# coding: utf-8

# In[1]:


import tensorflow as tf
import tensorflow_hub as hub

import numpy as np
import pandas as pd
from PIL import Image

import os
import glob
import natsort 

import cv2
import time
import math


# 전환된 이미지파일 저장할 경로
imgs_path = './img'  


# 테스트 비디오 경로
test_video_path = './test.mp4'  


"""
toImages(): 
    동영상을 이미지로 변환해주는 함수.

Args:
    - img_path: 변환할 이미지를 저장할 경로.
    - input_video_file: 이미지로 변환할 비디오 경로.

Returns:
    
"""
def toImages(img_path, input_video_file):

    cam = cv2.VideoCapture(input_video_file)
    counter = 0
    while True:
        flag, frame = cam.read()
        if flag:
            cv2.imwrite(os.path.join(img_path, str(counter) + '.jpg'),frame)
            counter = counter + 1
        else:
            break
        if cv2.waitKey(1) == 27:
            break
    cv2.destroyAllWindows()
    


"""
runDetector(): 
    이미지에서 객체를 탐지하는 함수(Faster-RCNN 적용).

Args:
    - detector: 객체 탐지 모듈
    - path: 모듈을 적용시킬 이미지 파일의 경로

Returns:
    - df: 프레임별 탐지된 객체들의 위치 정보가 담긴 DataFrame
    
"""
def runDetector(detector, path):
    
  df = pd.DataFrame(columns=['id', 'x', 'y'])
    
  img = tf.io.read_file(path)
  img = tf.image.decode_jpeg(img, channels=3)

  converted_img  = tf.image.convert_image_dtype(img, tf.float32)[tf.newaxis, ...]
  result = detector(converted_img)

  result = {key:value.numpy() for key,value in result.items()}
    
  max_boxes = 10
  min_score = 0.1

  boxes = result["detection_boxes"]
  class_names = result["detection_class_entities"]
  scores = result["detection_scores"]

    
  for i in range(min(boxes.shape[0], max_boxes)):
    
    if scores[i] >= min_score:
        
      image = Image.fromarray(np.uint8(img.numpy())).convert("RGB")
      ymin, xmin, ymax, xmax = tuple(boxes[i])
      im_width, im_height = image.size
      (left, right, top, bottom) = (xmin * im_width, xmax * im_width,
                                    ymin * im_height, ymax * im_height)
    
      display_str = [class_names[i].decode("ascii"), int(100 * scores[i])]
  
      if display_str[0] == 'Person':
        df = df.append({'id' : display_str[0], 'x' : float(left+right)/2.0, 'y' : float(top+bottom)/2.0}, ignore_index=True)
  return df



def distance(x1, y1, x2, y2):
    return math.sqrt((x1-x2)**2 + (y1-y2)**2)


"""
objectIndexing(): 
    같은 Class에 속하는 객체들을 구분하여 id값을 지정해주는 함수

Args:
    - output_df: run_detector()의 return 값으로 전달된 DataFrame

Returns:
    - 객체들의 id 값이 담긴 list
    
"""

def objectIndexing(output_df):
    count_num = 0

    for i in range(len(output_df.index)):
        if output_df[0][i] == 'frame':
          exec("frame%d = pd.DataFrame(columns=['id', 'x', 'y'])" % count_num)
          count_num += 1
        elif count_num <= 1:
          exec("frame%d = frame%d.append({'id' : %d, 'x' : output_df[1][%d], 'y' : output_df[2][%d]}, ignore_index=True)" % (count_num-1, count_num-1, i, i, i))
        else:
          exec("frame%d = frame%d.append({'id' : 0, 'x' : output_df[1][%d], 'y' : output_df[2][%d]}, ignore_index=True)" % (count_num-1, count_num-1, i, i))
        
        
    max_id = 0
    for i in range(count_num):
      exec("id = len(frame%d.index)" % i)
      globals().update(locals())
      if max_id < id:
          max_id = id

    return_id_list = ['frame']
    exec("return_id_list += list(frame%d['id'])" % 0)

    
    # 프레임 수 -1번 반복
    for frameNum in range(count_num-1):
        exec("beforeF = frame%d" % int(frameNum))
        exec("afterF = frame%d" % int(frameNum+1))
        globals().update(locals())
        

        # 객체의 수 변화X
        if len(beforeF.index) == len(afterF.index):
          afteridx = list(range(len(afterF.index)))
          for beforeIdx in range(len(beforeF.index)):
              x = beforeF['x'][beforeIdx]
              y = beforeF['y'][beforeIdx]
              minidx = -1
              mindis = 999
              for afterCount in afteridx:
                dis = distance(x, y, afterF['x'][afterCount], afterF['y'][afterCount])
                if mindis > dis:
                  minidx = afterCount
                  mindis = dis
              afterF['id'][minidx] = beforeF['id'][beforeIdx]
              afteridx.remove(minidx)

        # 객체의 수 감소
        elif len(beforeF.index) > len(afterF.index):
            beforeidx = list(range(len(beforeF.index)))
            for afterIdx in range(len(afterF.index)):
                x = afterF['x'][afterIdx]
                y = afterF['y'][afterIdx]
                minidx = -1
                mindis = 999
                for beforeCount in beforeidx:
                  dis = distance(x, y, beforeF['x'][beforeCount], beforeF['y'][beforeCount])
                  if mindis > dis:
                    minidx = beforeCount
                    mindis = dis
                    
                afterF['id'][afterIdx] = beforeF['id'][minidx]
                beforeidx.remove(minidx)      
            

        # 객체의 수 증가    
        else:
            afteridx = list(range(len(afterF.index)))
            for beforeIdx in range(len(beforeF.index)):
                x = beforeF['x'][beforeIdx]
                y = beforeF['y'][beforeIdx]
                minidx = -1
                mindis = 999
                for afterCount in afteridx:
                  dis = distance(x, y, afterF['x'][afterCount], afterF['y'][afterCount])
                  if mindis > dis:
                    minidx = afterCount
                    mindis = dis
                afterF['id'][minidx] = beforeF['id'][beforeIdx]
                afteridx.remove(minidx) 

            for i in range(len(afterF.index)):
              if afterF['id'][i] == 0.0:
                afterF['id'][i] = max_id
                max_id += 1

        return_id_list += ['frame']
        exec("return_id_list += list(afterF['id'])")

    return return_id_list


# In[5]:


# start = time.time()

# conver to imgs
toImages(imgs_path, test_video_path)


# module load
module_handle = "https://tfhub.dev/google/faster_rcnn/openimages_v4/inception_resnet_v2/1"
detector = hub.load(module_handle).signatures['default']

# imgs load
imgs= glob.glob(imgs_path+'/*.jpg')
imgs =  natsort.natsorted(imgs)

final_df = pd.DataFrame(columns=['id', 'x', 'y'])
frame_count = 0

# run detector
for file in imgs:
    frame_df = runDetector(detector, file)
    final_df = final_df.append({'id' : 'frame', 'x' : frame_count, 'y' : len(frame_df.index)}, ignore_index = True)
    final_df = final_df.append(frame_df, ignore_index=True)
    frame_count += 1

# save df to txt
final_df.to_csv('output.txt', sep='\t', index=False, header=False)

# load txt to df
final_df = pd.read_csv('output.txt', sep='\t', header=None)


# 객체 구분 알고리즘
id_list = objectIndexing(final_df)

final_df[0] = id_list
final_df.to_csv('../Resources/reality_data.txt', sep='\t', index=False, header=False)

# end = time.time()

# print('time : ', int(end-start)/60)


# In[ ]:




