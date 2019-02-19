pipeline {
  agent any
  stages {
    stage('error') {
      steps {
        echo '111111'
        sh '/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod ProjectBuilder.BuildIos'
      }
    }
  }
}