MVC3 Plupload To Amazon S3
===================================================

What is this example
-----------------
Have 2 sample method to send files to a S3.

First method (Direct send using plupload)
-------------------------------
Like this:
'$("#uploader").plupload({'
        '//amazon settings.'
        'runtimes: "flash,silverlight",'
        'url: "http://" + $('#Bucket').val() + ".s3.amazonaws.com/",
        max_file_size: '2mb',
		

Second method (From controller)
-------------------------------
Like this:
'using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client())'
'{'
'	// simple object put'
'	PutObjectRequest request = new PutObjectRequest();'
'	request.WithBucketName(ConfigurationManager.AppSettings["bucketname"]).WithKey(keyname).WithInputStream(pObject);'
''
'	using (client.PutObject(request)) { }'
'}'
		
