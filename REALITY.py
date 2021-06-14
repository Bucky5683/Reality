#!/usr/bin/env python
# coding: utf-8

# In[3]:

import logging

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
imgs_path = 'D:/REALITY/unity/REALITY/Assets/Script/imgfolder'


# 테스트 비디오 경로
test_video_path = 'D:/REALITY/unity/REALITY/Assets/Resource/test.mp4'


# output file 저장 경로
output_path = 'D:/REALITY/unity/REALITY/Assets/Script/outputfolder'


# 최종 파일 경로 + 이름
final_path = 'D:/REALITY/unity/REALITY/Assets/Resources/reality_data.txt'


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
    
  max_boxes = 30
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
    - beforeF, afterF : 연이은 프레임의 객체 인식 정보
    - max_id : id의 최대값

Returns:
    - [해당 프레임 객체들의 id 값이 담긴 list, 업데이트 된 max_id]
    
"""
def objectIndexing(beforeF, afterF, max_id):
    
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
          if afterF['id'][i] == 'Person':
            afterF['id'][i] = max_id
            max_id += 1

    return_list = [afterF['id'], max_id]
    return return_list


# In[ ]:


start = time.time()
'''
logging.warning(str(test_video_path))

# conver to imgs
toImages(imgs_path, test_video_path)
logging.warning('Finish toImages')

# module load
module_handle = "https://tfhub.dev/google/faster_rcnn/openimages_v4/inception_resnet_v2/1"
detector = hub.load(module_handle).signatures['default']
logging.warning('Finish module load')

# imgs load
imgs= glob.glob(imgs_path+'/*.jpg')
imgs =  natsort.natsorted(imgs)

final_df = pd.DataFrame(columns=['id', 'x', 'y'])
frame_count = 0

logging.warning('Start runDetector')
# run detector
for file in imgs:
    final_df = pd.DataFrame(columns=['id', 'x', 'y'])
    frame_df = runDetector(detector, file)
    final_df = final_df.append(frame_df, ignore_index=True)
    name = output_path + '/' + str(frame_count) + '.txt'
    final_df.to_csv(name, sep='\t', index=False, header=False)

    logging.warning('ok')
    frame_count += 1

'''
# load output files
logging.warning('start load output')
outputs_path = output_path + '/*.txt'
outputs = glob.glob(outputs_path)
outputs =  natsort.natsorted(outputs)


final_df = pd.DataFrame(columns=['id', 'x', 'y'])
max_id = 0
frame = 0
before = pd.DataFrame(columns=['id', 'x', 'y'])

logging.warning('start indexing')
for frame in range(len(outputs)-1):
    before_path = outputs[frame]
    after_path = outputs[frame+1]
    if frame==0:
        before = pd.read_csv(before_path, sep='\t', names = ['id', 'x', 'y'], header=None)
        id_list = np.array(range(1, len(before.index)+1))
        before['id'] = id_list
        max_id = len(before.index)
    
    final_df = final_df.append({'id' : 'frame', 'x' : frame, 'y' : len(before.index)}, ignore_index=True)    
    final_df = final_df.append(before, ignore_index=True)
    
    after = pd.read_csv(after_path, sep='\t', names = ['id', 'x', 'y'], header=None)
    [after['id'], max_id] = objectIndexing(before, after, max_id)
    
    before = after
    
# save final txt file 
final_df = final_df.append({'id' : 'frame', 'x' : (frame+1), 'y' : len(before.index)}, ignore_index=True)    
final_df = final_df.append(before, ignore_index=True)
final_df.to_csv(final_path, sep='\t', header=False, index=False)   


end = time.time()

logging.warning(int(end-start)/60)


# In[ ]:




