﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using FitAndGym.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FitAndGym.ViewModels
{
    public class FitAndGymViewModel : INotifyPropertyChanged
    {
        // Constructor & DataContext initialization
        private FitAndGymDataContext db;

        public FitAndGymViewModel() { } // to fulfill Design-Time data access requirements
        public FitAndGymViewModel(string fitAndGymConnectionString)
        {
            db = new FitAndGymDataContext(fitAndGymConnectionString);
            db.Log = new Utilities.DebugTextWriter();
        }

        #region Application's DB Collections

        private ObservableCollection<TrainingDay> _trainingDays;
        public ObservableCollection<TrainingDay> TrainingDays
        {
            get { return _trainingDays; }
            set
            {
                _trainingDays = value;
                NotifyPropertyChanged("TrainingDays");
            }
        }

        private ObservableCollection<Exercise> _exercises;
        public ObservableCollection<Exercise> Exercises
        {
            get { return _exercises; }
            set
            {
                _exercises = value;
                NotifyPropertyChanged("Exercises");
            }
        }

        #endregion

        public async Task LoadTrainingDaysCollectionFromDatabase()
        {
            await Task.Factory.StartNew(() =>
            {
                var trainingDaysInDB = from TrainingDay trDay
                                       in db.TrainingDays
                                       select trDay;

                TrainingDays = new ObservableCollection<TrainingDay>(trainingDaysInDB);
            });
        }

        public async Task LoadExercisesCollectionFromDatabase()
        {
            await Task.Factory.StartNew(() =>
            {
                var exercisesInDB = from Exercise ex
                                    in db.Exercises
                                    select ex;

                Exercises = new ObservableCollection<Exercise>(exercisesInDB);
            });
        }
        
        public void AddNewExercise(Exercise exercise)
        {
            Exercises.Add(exercise);

            db.Exercises.InsertOnSubmit(exercise);
            db.SubmitChanges();
        }

        public void UpdateExercise(Exercise exerciseToUpdate)
        {
            Exercise exToUpdate = db.Exercises.FirstOrDefault(ex => ex.ExerciseId == exerciseToUpdate.ExerciseId);

            if (exToUpdate == null) throw new Exception("Exercise to edit not found - from UpdateExercise");

            exToUpdate.AmountOfReps = exerciseToUpdate.AmountOfReps;
            exToUpdate.AmountOfSets = exerciseToUpdate.AmountOfSets;
            exToUpdate.DurationInMinutes = exerciseToUpdate.DurationInMinutes;
            exToUpdate.ExerciseName = exerciseToUpdate.ExerciseName;
            exToUpdate.ImageUri = exerciseToUpdate.ImageUri;
            exToUpdate.Intensity = exerciseToUpdate.Intensity;
            exToUpdate.OtherInfo = exerciseToUpdate.OtherInfo;

            db.SubmitChanges();
        }

        public Exercise GetExerciseById(int exId)
        {
            return db.Exercises.FirstOrDefault(ex => ex.ExerciseId == exId);
        }

        public TrainingDay GetTrainingById(int trId)
        {
            return db.TrainingDays.FirstOrDefault(tr => tr.TrainingDayId == trId);
        }

        public void DeleteExercise(Exercise exerciseToDelete)
        {
            Exercise exToDelete = GetExerciseById(exerciseToDelete.ExerciseId);
            if (exToDelete == null) throw new Exception("Exercise to delete not found - from DeleteExercise");

            Exercises.Remove(exToDelete);
            db.Exercises.DeleteOnSubmit(exToDelete);

            db.ExTrDayConnectors.DeleteAllOnSubmit(
                db.ExTrDayConnectors.Where(x =>
                    x._exerciseId == exerciseToDelete.ExerciseId));

            db.SubmitChanges();
        }

        public void DeleteTraining(TrainingDay trainingToDelete)
        {
            TrainingDay trToDelete = GetTrainingById(trainingToDelete.TrainingDayId);
            if (trToDelete == null) throw new Exception("Training to delete not found - from DeleteTraining");

            TrainingDays.Remove(trToDelete);
            db.TrainingDays.DeleteOnSubmit(trToDelete);

            db.ExTrDayConnectors.DeleteAllOnSubmit(
                db.ExTrDayConnectors.Where(x =>
                    x._trainingDayId == trainingToDelete.TrainingDayId));

            db.SubmitChanges();
        }

        #region Events Stuff

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        internal void UpdateTraining(TrainingDay trainingToUpdate)
        {
            TrainingDay trToUpdate = GetTrainingById(trainingToUpdate.TrainingDayId);
            if (trToUpdate == null) throw new Exception("Training to edit not found - from UpdateTraining");

            TrainingDays.Remove(trToUpdate);
            db.TrainingDays.DeleteOnSubmit(trToUpdate);

            db.ExTrDayConnectors.DeleteAllOnSubmit(
                db.ExTrDayConnectors.Where(x =>
                    x._trainingDayId == trainingToUpdate.TrainingDayId));

            TrainingDays.Add(trainingToUpdate);
            trainingToUpdate.TrainingDayId = 0;
            db.TrainingDays.InsertOnSubmit(trainingToUpdate);

            db.SubmitChanges();
        }

        internal void AddNewTraining(TrainingDay newTraining)
        {
            TrainingDays.Add(newTraining);

            db.TrainingDays.InsertOnSubmit(newTraining);
            db.SubmitChanges();
        }
    }
}
