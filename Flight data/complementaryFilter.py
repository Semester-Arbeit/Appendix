class complementaryFilter:

    def __init__(self, alpha):
        self.last_executionTime = None
        self.alpha = alpha
        self.speed_est_integrated = 0
        self.speed_est_differentiator = 0
        self.speed_est = 0
        self.last_pos_value = 0

    def newEstimate(self,time, a, p, staticAccOffset = 0):
        if (self.last_executionTime == None):
            self.last_executionTime = time
            self.last_pos_value = p
            return 0

        delta_t = ((time - self.last_executionTime)/1000000)

        self.speed_est_integrated = self.speed_est + (a - staticAccOffset) * delta_t
        self.speed_est_differentiator = (p - self.last_pos_value)/delta_t

        self.speed_est = (1-self.alpha) * self.speed_est_integrated + (self.alpha) * self.speed_est_differentiator

        self.last_pos_value = p
        self.last_executionTime = time

        return self.speed_est